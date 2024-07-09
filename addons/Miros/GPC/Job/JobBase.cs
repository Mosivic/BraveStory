using System;
using System.Collections.Generic;
using GPC.States;

namespace GPC.Job;

public abstract class JobBase(AbsState state) : AbsJob(state), IJob
{
    public virtual void Start()
    {
        State.Status = JobRunningStatus.Running;
        _Start();
    }

    public void Over()
    {
        _Over();
    }
    

    public virtual void Pause()
    {
        State.Status = JobRunningStatus.NoRun;
        _Pause();
    }

    public virtual void Resume()
    {
        State.Status = JobRunningStatus.Running;
        _Resume();
    }

    public virtual void Stack(AbsState stackState)
    {
        switch (state.StackType)
        {
            case StateStackType.Source:
                state.StackSourceCountDict ??= new Dictionary<IGpcToken, int>
                    { { state.Source, 1 } };

                if (!state.StackSourceCountDict.ContainsKey(stackState.Source))
                {
                    state.StackSourceCountDict.Add(stackState.Source,1);
                    //Stack
                }
                else if(state.StackSourceCountDict[state.Source] < state.StackMaxCount)
                {
                    state.StackSourceCountDict.Add(state.Source,1);
                    //Stack
                }
                else
                    //StackOverflow
                break;

                break;
            case StateStackType.Target:
                if (state.StackCurrentCount < state.StackMaxCount)
                {
                    state.StackCurrentCount += 1;
                    //Stack
                }
                else
                   //StackOverflow
                
                break;

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _Stack();
    }
    
    protected virtual void OnSucceed()
    {
        State.Status = JobRunningStatus.Succeed;
        _OnSucceed();
    }
    
    protected virtual void OnFailed()
    {
        State.Status = JobRunningStatus.Failed;
        _OnFailed();
    }
    
    protected virtual void OnStackOverflow(AbsState state)
    {
        _OnStackOverflow();
    }
    
    protected virtual void OnExpiration()
    {
        State.Status = JobRunningStatus.Succeed;
        State.DurationElapsed = 0;
        _OnStackExpiration();
    }
    
    protected virtual void OnPeriod()
    {
        State.PeriodElapsed = 0;
        _OnPeriod();
    }
    
    public virtual bool IsPrepared()
    {
        return _IsPrepared();
    }
    
    protected virtual bool IsSucceed()
    {
        return _IsSucceed();
    }
    
    protected virtual bool IsFailed()
    {
        return _IsFailed();
    }
    
    public virtual void Update(double delta)
    {
        if (IsFailed()) OnFailed();
        if (IsSucceed()) OnSucceed();

        State.DurationElapsed += delta;
        State.PeriodElapsed += delta;
        
        if (State.Duration > 0 && State.DurationElapsed > State.Duration)
        {
            OnExpiration();
        }
        if (State.Period > 0 && State.PeriodElapsed > State.Period)
        {
            OnPeriod();
        }

        _Update(delta);
    }

    public virtual void PhysicsUpdate(double delta)
    {
        _PhysicsUpdate(delta);
    }


}