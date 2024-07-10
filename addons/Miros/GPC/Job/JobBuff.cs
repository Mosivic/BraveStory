using System;
using System.Collections.Generic;
using GPC.States;
using GPC.States.Buff;

namespace GPC.Job;

public class JobBuff(Buff buff) : JobBase(buff)
{
    public override void Enter()
    {
        if (buff.DurationPolicy == BuffDurationPolicy.Instant)
        {
            ApplyModifiers();
            buff.Status = JobRunningStatus.Succeed;
        }
        else if (buff.DurationPolicy == BuffDurationPolicy.Infinite)
        {
            if(buff.Period > 0 && buff.IsExecutePeriodicEffectOnStart == false) return;
            ApplyModifiers();
        }
        else if (buff.DurationPolicy == BuffDurationPolicy.Duration)
        {
            if(buff.Period > 0 && buff.IsExecutePeriodicEffectOnStart == false) return;
            ApplyModifiers();
        }
        
        base.Enter();
    }
    
    public override void Resume()
    {
        buff.Status = JobRunningStatus.Running;
        if (buff.PeriodicInhibitionPolicy == BuffPeriodicInhibitionPolicy.Reset)
        {
            buff.PeriodElapsed = 0;
        }
        else if(buff.PeriodicInhibitionPolicy == BuffPeriodicInhibitionPolicy.ExecuteAndReset)
        {
            ApplyModifiers();
            buff.PeriodElapsed = 0;
        }
        base.Resume();
    }
    
    public override void Stack(AbsState stackState)
    {
        if (buff.StackIsRefreshDuration)
            buff.DurationElapsed = 0;
        if (buff.StackIsResetPeriod)
            buff.PeriodElapsed = 0;

        base.Stack(stackState);
    }

    protected override void OnSucceed()
    {
        if (buff.DurationPolicy == BuffDurationPolicy.Instant)
        {

        }
        else if (buff.DurationPolicy == BuffDurationPolicy.Infinite)
        {
            CancelModifiers();
        }
        else if (buff.DurationPolicy == BuffDurationPolicy.Duration)
        {

        }
        base.OnSucceed();
    }

    protected override void OnFailed()
    {
        switch (buff.DurationPolicy)
        {
            case BuffDurationPolicy.Instant:
                break;
            case BuffDurationPolicy.Infinite:
                CancelModifiers();
                break;
            case BuffDurationPolicy.Duration:
                break;
        }

        base.OnFailed();
    }

    protected override void OnPeriod()
    {
        ApplyModifiers();
        base.OnPeriod();
    }
    
    

    protected override void OnStackOverflow()
    {
        base.OnStackOverflow();
    }

    protected override void OnStackExpiration()
    {
        base.OnStackExpiration();
    }

    private void ApplyModifiers()
    {
        var random = new Random();
        if(random.NextDouble() > buff.Chance) return;
        
        for (var i = 0; i < buff.Modifiers.Count; i++)
        {
            var modifier = buff.Modifiers[i];
            modifier.Recored = modifier.Property.Value;
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
            }
        }
    }

    private void CancelModifiers()
    {
        foreach (var modifier in buff.Modifiers)
        {
            modifier.Property.Value = modifier.Recored;
        }
    }
}