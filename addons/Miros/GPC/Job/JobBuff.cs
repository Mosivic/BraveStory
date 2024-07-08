﻿using Godot;
using GPC.States;
using GPC.States.Buff;

namespace GPC.Job;

public class JobBuff(Buff buff) : JobBase(buff)
{
    public override void Enter()
    {
        State.IsRunning = true;
        State.RunningResult = JobRunningResult.NoResult;
        
        switch (buff.DurationPolicy)
        {
            case BuffDurationPolicy.Instand:
                ApplyModifiers();
                State.IsRunning = false;
                State.RunningResult = JobRunningResult.Succeed;
                break;
            case BuffDurationPolicy.Infinite:
                ApplyModifiers();
                break;
            case BuffDurationPolicy.Interval:
                break;
        }
    }

    public override void Exit()
    {
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

    public override void Break()
    {
        State.IsRunning = false;
        State.RunningResult = JobRunningResult.Failed;
        
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