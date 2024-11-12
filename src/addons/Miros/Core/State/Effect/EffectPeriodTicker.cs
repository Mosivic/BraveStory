
using System;
using Godot;

namespace Miros.Core;

public class EffectPeriodTicker
{
    private double _periodRemaining;
    private double _durationRemaining;
    private readonly Effect _effect;

    public EffectPeriodTicker(Effect effect)
    {
        _effect = effect;
        _periodRemaining = Period;
        _durationRemaining = Duration;
    }

    private double Period => _effect.Period;
    private double Duration => _effect.Duration;
    private DurationPolicy DurationPolicy => _effect.DurationPolicy;
    private StackingType StackingType => _effect.Stacking.StackingType;
    private ExpirationPolicy ExpirationPolicy => _effect.Stacking.ExpirationPolicy;
    private int StackCount => _effect.StackCount;

    public void Tick(double delta)
    {
        //_effect.PeriodExecution?.TriggerOnTick();
        UpdateDuration(delta);
        UpdatePeriod(delta);
    

        if (DurationPolicy == DurationPolicy.Duration && _durationRemaining<= 0)
        {
            // 处理STACKING
            if (StackingType == StackingType.None)
            {
                _effect.Status = RunningStatus.Succeed;
            }
            else
            {
                if (ExpirationPolicy == ExpirationPolicy.ClearEntireStack)
                {
                    _effect.Status = RunningStatus.Succeed;
                }
                else if (ExpirationPolicy == ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                {
                    if (StackCount > 1)
                    {
                        RefreshStack(StackCount - 1);
                    }
                    else
                    {
                        _effect.Status = RunningStatus.Succeed;
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

        if (_periodRemaining < Mathf.Epsilon)
        {
            // 第一次执行
            return;
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


    private bool RefreshStack()
    {
        var oldStackCount = _effect.StackCount;
        RefreshStack(_effect.StackCount + 1);
        _effect.RaiseOnStackCountChanged(oldStackCount, _effect.StackCount);
        return oldStackCount != _effect.StackCount;
    }

    private void RefreshStack(int stackCount)
    {
        if (stackCount <= _effect.Stacking.LimitCount)
        {
            // 更新栈数
            _effect.StackCount = Mathf.Max(1, stackCount); // 最小层数为1
                                                          // 是否刷新Duration
            if (_effect.Stacking.DurationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
            {
                _effect.PeriodTicker.ResetDuration();
            }
            // 是否重置Period
            if (_effect.Stacking.PeriodResetPolicy == PeriodResetPolicy.ResetOnSuccessfulApplication)
            {
                _effect.PeriodTicker.ResetPeriod();
            }
        }
        else
        {
            // 溢出GE生效
            foreach (var overflowEffect in _effect.Stacking.OverflowEffects)
                _effect.Owner.AddState(overflowEffect);

            if (_effect.Stacking.DurationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
            {
                if (_effect.Stacking.DenyOverflowApplication)
                {
                    //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
                    if (_effect.Stacking.ClearStackOnOverflow)
                    {
                        _effect.Status = RunningStatus.Succeed;
                    }
                }
                else
                {
                    _effect.PeriodTicker.ResetDuration();
                }
            }
        }
    }
}
