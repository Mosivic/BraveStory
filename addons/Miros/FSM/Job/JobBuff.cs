using System;
using FSM.States.Buff;

namespace FSM.Job;

public class JobBuff(BuffState buffState) : JobBase(buffState)
{
    public override void Enter()
    {
        if (buffState.DurationPolicy == BuffDurationPolicy.Instant)
        {
            ApplyModifiers();
            buffState.Status = JobRunningStatus.Succeed;
        }
        else if (buffState.DurationPolicy == BuffDurationPolicy.Infinite)
        {
            if (buffState.Period > 0 && buffState.IsExecutePeriodicEffectOnStart == false) return;
            ApplyModifiers();
        }
        else if (buffState.DurationPolicy == BuffDurationPolicy.Duration)
        {
            if (buffState.Period > 0 && buffState.IsExecutePeriodicEffectOnStart == false) return;
            ApplyModifiers();
        }

        base.Enter();
    }

    public override void Resume()
    {
        buffState.Status = JobRunningStatus.Running;
        if (buffState.PeriodicInhibitionPolicy == BuffPeriodicInhibitionPolicy.Reset)
        {
            buffState.PeriodElapsed = 0;
        }
        else if (buffState.PeriodicInhibitionPolicy == BuffPeriodicInhibitionPolicy.ExecuteAndReset)
        {
            ApplyModifiers();
            buffState.PeriodElapsed = 0;
        }

        base.Resume();
    }

    public override void Stack(IGpcToken source)
    {
        if (buffState.StackIsRefreshDuration)
            buffState.DurationElapsed = 0;
        if (buffState.StackIsResetPeriod)
            buffState.PeriodElapsed = 0;

        base.Stack(source);
    }

    protected override void OnSucceed()
    {
        switch (buffState.DurationPolicy)
        {
            case BuffDurationPolicy.Instant:
                break;
            case BuffDurationPolicy.Infinite:
                CancelModifiers();
                break;
            case BuffDurationPolicy.Duration:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        base.OnSucceed();
    }

    protected override void OnFailed()
    {
        switch (buffState.DurationPolicy)
        {
            case BuffDurationPolicy.Instant:
                break;
            case BuffDurationPolicy.Infinite:
                CancelModifiers();
                break;
            case BuffDurationPolicy.Duration:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        base.OnFailed();
    }

    protected override void OnPeriodOver()
    {
        ApplyModifiers();
        base.OnPeriodOver();
    }

    protected override void OnStack()
    {
        base.OnStack();
    }

    protected override void OnStackOverflow()
    {
        base.OnStackOverflow();
    }

    protected override void OnDurationOver()
    {
        base.OnDurationOver();
    }

    private void ApplyModifiers()
    {
        if (buffState.HasChance && new Random().NextDouble() > buffState.Chance)
            return;

        for (var i = 0; i < buffState.Modifiers.Count; i++)
        {
            var modifier = buffState.Modifiers[i];
            modifier.Record = modifier.Property.Value;
            switch (modifier.Operator)
            {
                case BuffModifierOperator.Add:
                    modifier.Property.Value += modifier.Affect;
                    break;
                case BuffModifierOperator.Multiply:
                    modifier.Property.Value *= modifier.Affect;
                    break;
                case BuffModifierOperator.Divide:
                    modifier.Property.Value /= modifier.Affect;
                    break;
                case BuffModifierOperator.Override:
                    modifier.Property.Value = modifier.Affect;
                    break;
                case BuffModifierOperator.Invalid:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            buffState.OnApplyModifierFunc?.Invoke(modifier);
        }
    }

    private void CancelModifiers()
    {
        foreach (var modifier in buffState.Modifiers)
        {
            modifier.Property.Value = modifier.Record;
            buffState.OnCancelModifierFunc?.Invoke(modifier);
        }
    }
}