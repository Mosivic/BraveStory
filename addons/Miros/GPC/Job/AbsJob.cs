using GPC.States;

namespace GPC.Job;

public abstract class AbsJob(AbsState state)
{
    protected readonly AbsState State = state;

    protected virtual void _Enter()
    {
        State.EnterFunc?.Invoke(State);
    }

    protected virtual void _Exit()
    {
        State.ExitFunc?.Invoke(State);
    }
    
    protected virtual void _Pause()
    {
        State.PauseFunc?.Invoke(State);
    }

    protected virtual void _Resume()
    {
        State.ResumeFunc?.Invoke(State);
    }
    
    protected virtual void _Stack(AbsState stackState)
    {
        State.StackFunc?.Invoke(State,stackState);
    }
    
    
    protected virtual bool _IsPrepared()
    {
        if (State.IsPreparedFunc != null)
            return State.IsPreparedFunc.Invoke();
        return true;
    }

    protected virtual void _OnSucceed()
    {
        State.OnSucceedFunc?.Invoke(State);
    }

    protected virtual void _OnFailed()
    {
        State.OnFailedFunc?.Invoke(State);
    }

    
    protected virtual void _OnPeriod()
    {
        State.PeriodFunc?.Invoke(State);
    }


    
    protected virtual void _OnStackOverflow()
    {
        State.OnStackOverflowFunc?.Invoke(State);
    }

    protected virtual void _OnStackExpiration()
    {
        State.OnStackExpirationFunc?.Invoke(State);
    }
    
    
    protected virtual bool _IsSucceed()
    {
        var isSucceed = false;
        if (State.IsSucceedFunc != null)
            isSucceed = State.IsSucceedFunc.Invoke();
        
        return isSucceed;
    }

    protected virtual bool _IsFailed()
    {
        var isFailed = false;
        
        if (State.IsFailedFunc != null)
        {
            isFailed = State.IsFailedFunc.Invoke();
        }
        else if (State.UsePrepareFuncAsRunCondition && State.IsPreparedFunc != null)
        {
            isFailed = !State.IsPreparedFunc.Invoke();
        }
        
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