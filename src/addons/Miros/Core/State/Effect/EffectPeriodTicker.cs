
using System;
using Godot;

namespace Miros.Core;

public class EffectPeriodTicker
{
    private float _periodRemaining;
    private readonly Effect _effect;

    public EffectPeriodTicker(Effect effect)
    {
        _effect = effect;
        _periodRemaining = Period;
    }

    private float Period => _effect.Period;

    public void Tick(double delta)
    {
        _effect.PeriodExecution?.TriggerOnTick();

        UpdatePeriod(delta);

        if (_effect.DurationPolicy == DurationPolicy.Duration && _effect.DurationRemaining() <= 0)
        {
            // 处理STACKING
            if (_effect.Stacking.StackingType == StackingType.None)
            {
                _effect.RemoveSelf();
            }
            else
            {
                if (_effect.Stacking.ExpirationPolicy == ExpirationPolicy.ClearEntireStack)
                {
                    _effect.RemoveSelf();
                }
                else if (_effect.Stacking.ExpirationPolicy == ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                {
                    if (_effect.StackCount > 1)
                    {
                        _effect.RefreshStack(_effect.StackCount - 1);
                        _effect.RefreshDuration();
                    }
                    else
                    {
                        _effect.RemoveSelf();
                    }
                }
                else if (_effect.Stacking.ExpirationPolicy == ExpirationPolicy.RefreshDuration)
                {
                    //持续时间结束时,再次刷新Duration，这相当于无限Duration，
                    _effect.RefreshDuration();
                }
            }
        }
    }

    /// <summary>
    /// 注意: Period 小于 0.01f 可能出现误差, 基本够用了
    /// </summary>
    private void UpdatePeriod(double delta)
    {
        // 前提: Period不会动态修改
        if (Period <= 0) return;

        var actualDuration = Time.GetTicksMsec() - _effect.ActivationTime;
        if (actualDuration < Mathf.Epsilon)
        {
            // 第一次执行
            return;
        }

        var dt = (float)delta;
        var excessDuration = actualDuration - _effect.Duration;
        if (excessDuration >= 0)
        {
            // 如果超出了持续时间，就减去超出的时间, 此时应该是最后一次执行
            dt -= excessDuration;
            // 为了避免误差, 保证最后一次边界得到执行机会
            dt += 0.0001f;
        }

        _periodRemaining -= dt;

        while (_periodRemaining < 0)
        {
            // 不能直接将_periodRemaining置为0, 这将累计误差
            _periodRemaining += Period;
            _effect.PeriodExecution?.TriggerOnExecute();
        }
    }

    public void ResetPeriod()
    {
        _periodRemaining = Period;
    }
}
