using System;
using System.Collections.Generic;

namespace Miros.Core;

public class JobBuff(Buff buff) : JobBase(buff)
{
    public override void Enter()
    {
        if (buff.DurationPolicy == DurationPolicy.Instant)
        {
            ApplyModifiers();
            buff.Status = RunningStatus.Succeed;
        }
        else if (buff.DurationPolicy == DurationPolicy.Infinite)
        {
            if (buff.Period > 0 && buff.IsExecutePeriodicEffectOnStart == false) return;
            ApplyModifiers();
        }
        else if (buff.DurationPolicy == DurationPolicy.Duration)
        {
            if (buff.Period > 0 && buff.IsExecutePeriodicEffectOnStart == false) return;
            ApplyModifiers();
        }

        buff.StackCurrentCount = buff.StackMaxCount;
        buff.PeriodElapsed = 0;
        buff.DurationElapsed = 0;
        base.Enter();
    }

    public override void Resume()
    {
        buff.Status = RunningStatus.Running;
        if (buff.PeriodicInhibitionPolicy == PeriodicInhibitionPolicy.Reset)
        {
            buff.PeriodElapsed = 0;
        }
        else if (buff.PeriodicInhibitionPolicy == PeriodicInhibitionPolicy.ExecuteAndReset)
        {
            ApplyModifiers();
            buff.PeriodElapsed = 0;
        }

        base.Resume();
    }

    public void Stack(object source)
    {
        if (buff.StackIsRefreshDuration)
            buff.DurationElapsed = 0;
        if (buff.StackIsResetPeriod)
            buff.PeriodElapsed = 0;

        switch (buff.StackType)
        {
            case StateStackType.Source:
                buff.StackSourceCountDict ??= new Dictionary<object, int>
                    { { buff.Source, 1 } };

                //Not have stackState in Dict
                if (buff.StackSourceCountDict.TryAdd(source, 1))
                {
                    buff.StackCurrentCount += 1;
                    OnStack();
                }
                //Have stackState in Dict AND stackStateCount less than maxCount
                else if (buff.StackSourceCountDict[buff.Source] < buff.StackMaxCount)
                {
                    buff.StackSourceCountDict.Add(source, 1);
                    buff.StackCurrentCount += 1;
                    OnStack();
                }
                //Have stackState in Dict AND Overflow
                else
                {
                    OnStackOverflow();
                }

                break;
            case StateStackType.Target:
                if (buff.StackCurrentCount < buff.StackMaxCount)
                {
                    buff.StackCurrentCount += 1;
                    OnStack();
                }
                else
                {
                    OnStackOverflow();
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void Succeed()
    {
        switch (buff.DurationPolicy)
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

        base.Succeed();
    }

    protected override void Failed()
    {
        switch (buff.DurationPolicy)
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

        base.Failed();
    }



    public override void Update(double delta)
    {
        if (buff.Status != RunningStatus.Running) return;
        buff.DurationElapsed += delta;
        buff.PeriodElapsed += delta;

        if (buff.Duration > 0 && buff.DurationElapsed > buff.Duration) OnDurationOver();
        if (buff.Period > 0 && buff.PeriodElapsed > buff.Period) OnPeriodOver();

        base.Update(delta);
    }


    protected virtual void OnStack()
    {

    }

    protected virtual void OnStackOverflow()
    {

    }

    protected virtual void OnDurationOver()
    {
        buff.StackCurrentCount -= 1;
        buff.DurationElapsed = 0;

        if (buff.StackCurrentCount == 0)
            buff.Status = RunningStatus.Succeed;


    }

    protected virtual void OnPeriodOver()
    {
        ApplyModifiers();
        buff.PeriodElapsed = 0;

    }

    private void ApplyModifiers()
    {
        if (buff.HasChance && new Random().NextDouble() > buff.Chance)
            return;

        // for (var i = 0; i < buff.Modifiers.Count; i++)
        // {
        //     var modifier = buff.Modifiers[i];
        //     modifier.Record = modifier.Property;
        //     switch (modifier.Operator)
        //     {
        //         case ModifierOperation.Add:
        //             modifier.Property += modifier.Affect;
        //             break;
        //         case ModifierOperation.Multiply:
        //             modifier.Property *= modifier.Affect;
        //             break;
        //         case ModifierOperation.Divide:
        //             modifier.Property /= modifier.Affect;
        //             break;
        //         case ModifierOperation.Override:
        //             modifier.Property = modifier.Affect;
        //             break;
        //         case ModifierOperation.Invalid:
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }

        //     buff.OnApplyModifierFunc?.Invoke(modifier);
        // }
    }
    private void CancelModifiers()
    {
        // foreach (var modifier in buff.Modifiers)
        // {
        //     modifier.Property = modifier.Record;
        //     buff.OnCancelModifierFunc?.Invoke(modifier);
        // }
    }



}