using System;
using System.Collections.Generic;
using FSM.States;

namespace FSM.Job;


public abstract class JobBase(AbsState state) : AbsJob(state), IJob
{
    public string Name => state.Name;
    public Layer Layer => state.Layer;
    public int Priority => state.Priority;
    public bool IsStack => state.IsStack;
    public object Source => state.Source;

    public virtual void Enter()
    {
        state.Status = JobRunningStatus.Running;
        state.StackCurrentCount = state.StackMaxCount;
        state.PeriodElapsed = 0;
        state.DurationElapsed = 0;
        _Enter();
    }

    public void Exit()
    {
        if (state.Status == JobRunningStatus.Running)
            OnFailed();
        _Exit();
    }


    public virtual void Pause()
    {
        state.Status = JobRunningStatus.NoRun;
        _Pause();
    }

    public virtual void Resume()
    {
        state.Status = JobRunningStatus.Running;
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
        return _IsPrepared();
    }

    public virtual bool CanExit()
    {
        if (state.Status is JobRunningStatus.Succeed or JobRunningStatus.Failed)
            return true;
        return false;
    }

    public virtual void Update(double delta)
    {
        if (state.Status != JobRunningStatus.Running) return;
        if (IsFailed()) OnSucceed();
        if (IsSucceed()) OnFailed();

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
        state.Status = JobRunningStatus.Succeed;
        _OnSucceed();
    }

    protected virtual void OnFailed()
    {
        state.Status = JobRunningStatus.Failed;
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
            state.Status = JobRunningStatus.Succeed;

        _OnDurationOVer();
    }

    protected virtual void OnPeriodOver()
    {
        state.PeriodElapsed = 0;
        _OnPeriodOver();
    }

    protected virtual bool IsSucceed()
    {
        return _IsSucceed();
    }

    protected virtual bool IsFailed()
    {
        return _IsFailed();
    }
}