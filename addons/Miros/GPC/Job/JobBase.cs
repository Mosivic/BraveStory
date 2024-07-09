using System;
using System.Collections.Generic;
using GPC.States;

namespace GPC.Job;

public abstract class JobBase(AbsState state) : AbsJob(state), IJob
{
    public virtual void Enter()
    {
        State.Status = JobRunningStatus.Running;
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
                
                //Not have stackState in Dict
                if (!state.StackSourceCountDict.ContainsKey(stackState.Source))
                {
                    state.StackSourceCountDict.Add(stackState.Source,1);
                    state.StackCurrentCount += 1;
                    OnStack();
                }
                //Have stackState in Dict AND stackStactCount less than maxCount
                else if(state.StackSourceCountDict[state.Source] < state.StackMaxCount)
                {
                    state.StackSourceCountDict.Add(stackState.Source,1);
                    state.StackCurrentCount += 1;
                    OnStack();
                }
                //Have stackState in Dict AND Overflow
                else
                    OnStackOverflow();
                break;
            case StateStackType.Target:
                if (state.StackCurrentCount < state.StackMaxCount)
                {
                    state.StackCurrentCount += 1;
                    OnStack();
                }
                else
                    OnStackOverflow();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _Stack(stackState);
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

    protected virtual void OnStack()
    {
        
    }
    
    protected virtual void OnStackOverflow()
    {
        _OnStackOverflow();
    }
    
    protected virtual void OnStackExpiration()
    {
        state.StackCurrentCount -= 1;
        state.DurationElapsed = 0;
        
        if (state.StackCurrentCount == 0)
            state.Status = JobRunningStatus.Succeed;
           
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
        if (IsFailed()) OnSucceed();
        if (IsSucceed()) OnFailed();

        State.DurationElapsed += delta;
        State.PeriodElapsed += delta;
        
        if (State.Duration > 0 && State.DurationElapsed > State.Duration)
        {
            OnStackExpiration();
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