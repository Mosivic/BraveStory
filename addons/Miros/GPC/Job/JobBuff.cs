﻿using Godot;
using GPC.States;
using GPC.States.Buff;

namespace GPC.Job;

public class JobBuff(Buff buff) : JobBase(buff)
{
    public override void Start()
    {
        State.Status = JobRunningStatus.Running;
        
        switch (buff.DurationPolicy)
        {
            case BuffDurationPolicy.Instand:
                ApplyModifiers();
                State.Status = JobRunningStatus.Succeed;
                break;
            case BuffDurationPolicy.Infinite:
                ApplyModifiers();
                break;
            case BuffDurationPolicy.Interval:
                break;
        }
    }

    public override void Succeed()
    {
        State.Status = JobRunningStatus.Succeed;
        
        switch (buff.DurationPolicy)
        {
            case BuffDurationPolicy.Instand:
                break;
            case BuffDurationPolicy.Infinite:
                CancelModifiers();
                break;
            case BuffDurationPolicy.Interval:
                break;
        }
    }

    public override void Failed()
    {
        State.Status = JobRunningStatus.Failed;
        
        switch (buff.DurationPolicy)
        {
            case BuffDurationPolicy.Instand:
                break;
            case BuffDurationPolicy.Infinite:
                CancelModifiers();
                break;
            case BuffDurationPolicy.Interval:
                break;
        }
    }


    private void ApplyModifiers()
    {
        for (int i = 0; i < buff.Modifiers.Count; i++)
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
        for (int i = 0; i < buff.Modifiers.Count; i++)
        {
            var modifier = buff.Modifiers[i];
            modifier.Property.Value = modifier.Recored;
        }
    }
}