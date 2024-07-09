using GPC.States;

namespace GPC.Job;

public abstract class AbsJob(AbsState state)
{
    protected readonly AbsState State = state;

    protected virtual void _Start()
    {
        State.StartFunc?.Invoke(State);
    }

    protected virtual void _Over()
    {
        
    }
    
    protected virtual void _Pause()
    {
        State.PauseFunc?.Invoke(State);
    }

    protected virtual void _Resume()
    {
        State.ResumeFunc?.Invoke(State);
    }
    
    protected virtual void _Stack()
    {
        State.StackFunc?.Invoke(State);
    }
    
    protected virtual bool _IsPrepared()
    {
        if (State.IsPreparedFunc != null)
            return State.IsPreparedFunc.Invoke();
        return true;
    }

    protected virtual void _OnSucceed()
    {
        State.SucceedFunc?.Invoke(State);
    }

    protected virtual void _OnFailed()
    {
        State.FailedFunc?.Invoke(State);
    }



    protected virtual void _OnPeriod()
    {
        State.PeriodFunc?.Invoke(State);
    }

    
    protected virtual void _OnStackOverflow()
    {
        State.StackOverflowFunc?.Invoke(State);
    }

    protected virtual void _OnStackExpiration()
    {
        State.StackExpirationFunc?.Invoke(State);
    }
    
    
    protected virtual bool _IsSucceed()
    {
        var isSucceed = false;
        if (State.IsSucceedFunc != null)
            isSucceed = State.IsSucceedFunc.Invoke();

        if (isSucceed)
            State.Status = JobRunningStatus.Succeed;
        return isSucceed;
    }

    protected virtual bool _IsFailed()
    {
        var isFailed = false;
        if (State.UsePrepareFuncAsRunCondition)
        {
            if (State.IsFailedFunc == null)
                isFailed = !State.IsPreparedFunc.Invoke();
            else
                isFailed = State.IsFailedFunc.Invoke();
        }
        else if (State.IsFailedFunc != null)
        {
            isFailed = State.IsFailedFunc.Invoke();
        }

        if (isFailed)
            State.Status = JobRunningStatus.Failed;
        return isFailed;
    }

    protected virtual void _PhysicsUpdate(double delta)
    {
        State.RunningPhysicsFunc?.Invoke(State);
    }

    protected virtual void _Update(double delta)
    {
        State.RunningFunc?.Invoke(State);
    }


}