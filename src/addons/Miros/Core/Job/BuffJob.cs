using System;
using System.Collections.Generic;

namespace Miros.Core;

public class JobBuff(Buff buffState) : NativeJob(buffState)
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

        buffState.StackCurrentCount = buffState.StackMaxCount;
        buffState.PeriodElapsed = 0;
        buffState.DurationElapsed = 0;
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

    public void Stack(object source)
    {
        if (buffState.StackIsRefreshDuration)
            buffState.DurationElapsed = 0;
        if (buffState.StackIsResetPeriod)
            buffState.PeriodElapsed = 0;

        switch (buffState.StackType)
        {
            case StateStackType.Source:
                buffState.StackSourceCountDict ??= new Dictionary<object, int>
                    { { buffState.Source, 1 } };

                //Not have stackState in Dict
                if (buffState.StackSourceCountDict.TryAdd(source, 1))
                {
                    buffState.StackCurrentCount += 1;
                    OnStack();
                }
                //Have stackState in Dict AND stackStateCount less than maxCount
                else if (buffState.StackSourceCountDict[buffState.Source] < buffState.StackMaxCount)
                {
                    buffState.StackSourceCountDict.Add(source, 1);
                    buffState.StackCurrentCount += 1;
                    OnStack();
                }
                //Have stackState in Dict AND Overflow
                else
                {
                    OnStackOverflow();
                }

                break;
            case StateStackType.Target:
                if (buffState.StackCurrentCount < buffState.StackMaxCount)
                {
                    buffState.StackCurrentCount += 1;
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

    protected override void OnFail()
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

        base.OnFail();
    }



    public override void Update(double delta)
    {
        if (state.Status != RunningStatus.Running) return;
        buffState.DurationElapsed += delta;
        buffState.PeriodElapsed += delta;

        if (buffState.Duration > 0 && buffState.DurationElapsed > buffState.Duration) OnDurationOver();
        if (buffState.Period > 0 && buffState.PeriodElapsed > buffState.Period) OnPeriodOver();

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
        buffState.StackCurrentCount -= 1;
        buffState.DurationElapsed = 0;

        if (buffState.StackCurrentCount == 0)
            buffState.Status = RunningStatus.Succeed;


    }

    protected virtual void OnPeriodOver()
    {
        ApplyModifiers();
        buffState.PeriodElapsed = 0;

    }

    private void ApplyModifiers()
    {
        if (buffState.HasChance && new Random().NextDouble() > buffState.Chance)
            return;

        // for (var i = 0; i < buffState.Modifiers.Count; i++)
        // {
        //     var modifier = buffState.Modifiers[i];
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

        //     buffState.OnApplyModifierFunc?.Invoke(modifier);
        // }
    }
    private void CancelModifiers()
    {
        // foreach (var modifier in buffState.Modifiers)
        // {
        //     modifier.Property = modifier.Record;
        //     buffState.OnCancelModifierFunc?.Invoke(modifier);
        // }
    }



}