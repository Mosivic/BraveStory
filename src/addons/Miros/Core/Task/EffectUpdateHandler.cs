using Godot;
using Miros.Utils;

namespace Miros.Core;

public class EffectUpdateHandler
{
    private readonly CounterTimer _durationTimer;
    private readonly Effect _effect;


    private readonly CounterTimer _periodTimer;

    public EffectUpdateHandler(Effect effect)
    {
        _effect = effect;

        _periodTimer = new CounterTimer(_effect.Period);
        _durationTimer = new CounterTimer(_effect.Duration);

        _periodTimer.Start();
        _durationTimer.Start();

        if (_effect.Period > 0)
            _periodTimer.OnTimerStop += OnPeriodOver;
        if (_effect.Duration > 0)
            _durationTimer.OnTimerStop += OnDurationOver;
    }

    public void Tick(double delta)
    {
        _durationTimer.Tick(delta);
        _periodTimer.Tick(delta);
    }

    private void OnPeriodOver()
    {
        _effect.OwnerAgent.ApplyModWithInstant(_effect);
        _effect.OwnerAgent.ApplyExecWithInstant(_effect);

        _periodTimer.Start();
    }

    private void OnDurationOver()
    {
        if (_effect.Stacking is null)
        {
            _effect.Status = RunningStatus.Succeed;
            return;
        }

        // 处理STACKING
        if (_effect.Stacking.StackingType == StackingType.None)
        {
            _effect.Status = RunningStatus.Succeed;
        }
        else
        {
            if (_effect.Stacking.ExpirationPolicy == ExpirationPolicy.ClearEntireStack)
            {
                _effect.Status = RunningStatus.Succeed;
            }
            else if (_effect.Stacking.ExpirationPolicy == ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
            {
                if (_effect.Stacking.StackCount > 1)
                    RefreshStack(_effect.Stacking.StackCount - 1);
                else
                    _effect.Status = RunningStatus.Succeed;
            }
            else if (_effect.Stacking.ExpirationPolicy == ExpirationPolicy.RefreshDuration)
            {
                //持续时间结束时,再次刷新Duration，这相当于无限Duration，
                _durationTimer.Start();
            }
        }
    }


    private void RefreshStack(int stackCount)
    {
        if (stackCount <= _effect.Stacking.LimitCount)
        {
            // 更新栈数
            _effect.Stacking.StackCount = Mathf.Max(1, stackCount); // 最小层数为1
            // 是否刷新Duration
            if (_effect.Stacking.DurationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
                _durationTimer.Start();
            // 是否重置Period
            if (_effect.Stacking.PeriodResetPolicy == PeriodResetPolicy.ResetOnSuccessfulApplication)
                _periodTimer.Start();
        }
        else
        {
            // 溢出GE生效
            foreach (var overflowEffect in _effect.Stacking.OverflowEffects)
                _effect.OwnerAgent.AddTaskFromState(ExecutorType.EffectExecutor, overflowEffect);

            if (_effect.Stacking.DurationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
            {
                if (_effect.Stacking.DenyOverflowApplication)
                {
                    //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
                    if (_effect.Stacking.ClearStackOnOverflow) _effect.Status = RunningStatus.Succeed;
                }
                else
                {
                    _durationTimer.Start();
                }
            }
        }
    }
}