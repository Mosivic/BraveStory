
using System;
using Godot;

namespace Miros.Core;

public class EffectPeriodTicker
{
    private double _periodRemaining;
    private double _durationRemaining;
    private readonly EffectJob _effectJob;
    private readonly Effect _effect;

    public EffectPeriodTicker(EffectJob effectJob, Effect effect)
    {
        _effectJob = effectJob;
        _effect = effect;
        _periodRemaining = Period;
    }

    private double Period => _effect.Period;
    private double ActivationTime => _effect.ActivationTime;
    private double Duration => _effect.Duration;
    private DurationPolicy DurationPolicy => _effect.DurationPolicy;
    private StackingType StackingType => _effect.Stacking.StackingType;
    private ExpirationPolicy ExpirationPolicy => _effect.Stacking.ExpirationPolicy;
    private int StackCount => _effect.StackCount;

    public void Tick(double delta)
    {
        //_effect.PeriodExecution?.TriggerOnTick();
        _effect.ActivationTime += delta;
        UpdateDuration(delta);
        UpdatePeriod(delta);
    

        if (DurationPolicy == DurationPolicy.Duration && _durationRemaining<= 0)
        {
            // 处理STACKING
            if (StackingType == StackingType.None)
            {
                _effectJob.RemoveSelf();
            }
            else
            {
                if (ExpirationPolicy == ExpirationPolicy.ClearEntireStack)
                {
                    _effectJob.RemoveSelf();
                }
                else if (ExpirationPolicy == ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                {
                    if (StackCount > 1)
                    {
                        _effectJob.RefreshStack(StackCount - 1);
                    }
                    else
                    {
                        _effectJob.RemoveSelf();
                    }
                }
                else if (ExpirationPolicy == ExpirationPolicy.RefreshDuration)
                {
                    //持续时间结束时,再次刷新Duration，这相当于无限Duration，
                    ResetDuration();
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

        if (ActivationTime < Mathf.Epsilon)
        {
            // 第一次执行
            return;
        }
        var excessDuration = ActivationTime - Duration;
        if (excessDuration >= 0)
        {
            // 如果超出了持续时间，就减去超出的时间, 此时应该是最后一次执行
            delta -= excessDuration;
            // 为了避免误差, 保证最后一次边界得到执行机会
            delta += 0.0001f;
        }

        _periodRemaining -= delta;

        while (_periodRemaining < 0)
        {
            // 不能直接将_periodRemaining置为0, 这将累计误差
            _periodRemaining += Period;
            //_effect.PeriodExecution?.TriggerOnExecute();
        }
    }

    private void UpdateDuration(double delta)
    {
        if (DurationPolicy == DurationPolicy.Infinite)
            return ;

        _durationRemaining -= delta;
    }

    public void ResetDuration()
    {
        _durationRemaining = Duration;
    }

    public void ResetPeriod()
    {
        _periodRemaining = Period;
    }
}
