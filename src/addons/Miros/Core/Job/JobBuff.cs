using System;

namespace Miros.Core;

public class JobBuff(Buff buffState) : AbsJobBase(buffState)
{
    public override void Enter()
    {
        if (buffState.DurationPolicy == DurationPolicy.Instant)
        {
            ApplyModifiers();
            buffState.Status = RunningStatus.Succeed;
        }
        else if (buffState.DurationPolicy == DurationPolicy.Infinite)
        {
            if (buffState.Period > 0 && buffState.IsExecutePeriodicEffectOnStart == false) return;
            ApplyModifiers();
        }
        else if (buffState.DurationPolicy == DurationPolicy.Duration)
        {
            if (buffState.Period > 0 && buffState.IsExecutePeriodicEffectOnStart == false) return;
            ApplyModifiers();
        }

        base.Enter();
    }

    public override void Resume()
    {
        buffState.Status = RunningStatus.Running;
        if (buffState.PeriodicInhibitionPolicy == PeriodicInhibitionPolicy.Reset)
        {
            buffState.PeriodElapsed = 0;
        }
        else if (buffState.PeriodicInhibitionPolicy == PeriodicInhibitionPolicy.ExecuteAndReset)
        {
            ApplyModifiers();
            buffState.PeriodElapsed = 0;
        }

        base.Resume();
    }

    public override void Stack(object source)
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
            case DurationPolicy.Instant:
                break;
            case DurationPolicy.Infinite:
                CancelModifiers();
                break;
            case DurationPolicy.Duration:
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
            case DurationPolicy.Instant:
                break;
            case DurationPolicy.Infinite:
                CancelModifiers();
                break;
            case DurationPolicy.Duration:
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
            modifier.Record = modifier.Property;
            switch (modifier.Operator)
            {
                case ModifierOperation.Add:
                    modifier.Property += modifier.Affect;
                    break;
                case ModifierOperation.Multiply:
                    modifier.Property *= modifier.Affect;
                    break;
                case ModifierOperation.Divide:
                    modifier.Property /= modifier.Affect;
                    break;
                case ModifierOperation.Override:
                    modifier.Property = modifier.Affect;
                    break;
                case ModifierOperation.Invalid:
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
            modifier.Property = modifier.Record;
            buffState.OnCancelModifierFunc?.Invoke(modifier);
        }
    }
}