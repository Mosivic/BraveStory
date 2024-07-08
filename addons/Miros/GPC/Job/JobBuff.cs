using System;
using GPC.States;
using GPC.States.Buff;

namespace GPC.Job;

public class JobBuff(Buff buff) : JobBase(buff)
{
    public override void OnStart()
    {
        buff.Status = JobRunningStatus.Running;

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
        _OnStart();
    }

    public override void OnSucceed()
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
        _OnSucceed();
    }

    public override void OnFailed()
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
        _OnFailed();
    }

    protected override void OnPeriod()
    {
        ApplyModifiers();
        _OnPeriod();
    }

    public override void OnPause()
    {
        _OnPause();
    }

    public override void OnResume()
    {
        if (buff.PeriodicInhibitionPolicy == BuffPeriodicInhibitionPolicy.Reset)
        {
            buff.PeriodElapsed = 0;
        }
        else if(buff.PeriodicInhibitionPolicy == BuffPeriodicInhibitionPolicy.ExecuteAndReset)
        {
             ApplyModifiers();
             buff.PeriodElapsed = 0;
        }
        _OnResume();
    }


    public override void OnStack()
    {
        
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