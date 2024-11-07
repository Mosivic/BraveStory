using System;
using System.Collections.Generic;


namespace Miros.Core;

public abstract class AbsJobBase : AbsJob, IJob
{
    private protected readonly AbsState state;

    protected AbsJobBase(AbsState state) : base(state)
    {
        this.state = state;
    }

    public AbsState State => state;

    public virtual void Enter()
    {
        state.Status = RunningStatus.Running;
        state.StackCurrentCount = state.StackMaxCount;
        state.PeriodElapsed = 0;
        state.DurationElapsed = 0;
        _Enter();
    }

    public void Exit()
    {
        if(CanExit())
            OnSucceed();
        else
            OnFailed();

        _Exit();
    }


    public virtual void Pause()
    {
        state.Status = RunningStatus.NoRun;
        _Pause();
    }

    public virtual void Resume()
    {
        state.Status = RunningStatus.Running;
        _Resume();
    }


    public virtual void Stack(object source)
    {
        switch (state.StackType)
        {
            case StateStackType.Source:
                state.StackSourceCountDict ??= new Dictionary<object, int>
                    { { state.Source, 1 } };

                //Not have stackState in Dict
                if (state.StackSourceCountDict.TryAdd(source, 1))
                {
                    state.StackCurrentCount += 1;
                    OnStack();
                }
                //Have stackState in Dict AND stackStateCount less than maxCount
                else if (state.StackSourceCountDict[state.Source] < state.StackMaxCount)
                {
                    state.StackSourceCountDict.Add(source, 1);
                    state.StackCurrentCount += 1;
                    OnStack();
                }
                //Have stackState in Dict AND Overflow
                else
                {
                    OnStackOverflow();
                }

                break;
            case StateStackType.Target:
                if (state.StackCurrentCount < state.StackMaxCount)
                {
                    state.StackCurrentCount += 1;
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

    public virtual bool CanEnter()
    {
        return state.EnterCondition?.Invoke(state) ?? true;
    }



    public virtual bool CanExit()
    {
        return state.ExitCondition?.Invoke(state) ?? true;
    }
    


    public virtual void Update(double delta)
    {
        if (state.Status != RunningStatus.Running) return;

        state.DurationElapsed += delta;
        state.PeriodElapsed += delta;

        if (state.Duration > 0 && state.DurationElapsed > state.Duration) OnDurationOver();
        if (state.Period > 0 && state.PeriodElapsed > state.Period) OnPeriodOver();

        _Update(delta);
    }

    public virtual void PhysicsUpdate(double delta)
    {
        _PhysicsUpdate(delta);
    }

    protected virtual void OnSucceed()
    {
        state.Status = RunningStatus.Succeed;
        _OnSucceed();
    }

    protected virtual void OnFailed()
    {
         state.Status = RunningStatus.Failed;
        _OnFailed();
    }

    protected virtual void OnStack()
    {
        _OnStack();
    }

    protected virtual void OnStackOverflow()
    {
        _OnStackOverflow();
    }

    protected virtual void OnDurationOver()
    {
        state.StackCurrentCount -= 1;
        state.DurationElapsed = 0;

        if (state.StackCurrentCount == 0)
            state.Status = RunningStatus.Succeed;

        _OnDurationOVer();
    }

    protected virtual void OnPeriodOver()
    {
        state.PeriodElapsed = 0;
        _OnPeriodOver();
    }

}