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

    protected virtual void _Break()
    {
        State.BreakFunc?.Invoke(State);
    }
    
    protected virtual void _Pause()
    {
        State.PauseFunc?.Invoke(State);
    }

    protected virtual void _Resume()
    {
        State.ResumeFunc?.Invoke(State);
    }

    protected virtual bool _IsPrepared()
    {
        if (State.IsPreparedFunc != null)
            return State.IsPreparedFunc.Invoke();
        return true;
    }

    protected virtual bool _IsSucceed()
    {
        var isSucceed = false;
        if (State.IsSucceedFunc != null)
            isSucceed =  State.IsSucceedFunc.Invoke();
        
        if (isSucceed)
            State.RunningResult = JobRunningResult.Succeed;
        return isSucceed;
    }

    protected virtual bool _IsFailed()
    {
        var isFailed = false;
        if (State.UsePrepareFuncAsRunCondition)
        {
            if (State.IsFailedFunc == null)
                isFailed =  !State.IsPreparedFunc.Invoke();
            else
                isFailed =  State.IsFailedFunc.Invoke();
        }
        else if(State.IsFailedFunc != null)
        {
            isFailed = State.IsFailedFunc.Invoke();
        }

        if (isFailed)
            State.RunningResult = JobRunningResult.Failed;
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

    protected virtual void _IntervalUpdate()
    {
        State.IntervalUpdateFunc?.Invoke(State);
    }
    
}