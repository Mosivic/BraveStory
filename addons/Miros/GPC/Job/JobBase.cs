using System;
using System.Collections.Generic;
using GPC.States;

namespace GPC.Job;

public abstract class JobBase(AbsState state) : AbsJob(state), IJob
{
    public virtual void Enter()
    {
        state.Status = JobRunningStatus.Running;
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
        state.PeriodElapsed = 0;
        _OnPeriod();
    }
    
    public virtual bool CanEnter()
    {
        return _IsPrepared();
    }

    public bool CanExit()
    {
        if (state.Status is JobRunningStatus.Succeed or JobRunningStatus.Failed)
            return true;
        return false;
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

        state.DurationElapsed += delta;
        state.PeriodElapsed += delta;
        
        if (state.Duration > 0 && state.DurationElapsed > state.Duration)
        {
            OnStackExpiration();
        }
        if (state.Period > 0 && state.PeriodElapsed > state.Period)
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