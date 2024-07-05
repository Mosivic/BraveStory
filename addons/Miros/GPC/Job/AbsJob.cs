using GPC.Scheduler;
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

    protected virtual bool _IsPrepared()
    {
        if (State.IsPreparedFunc != null)
            return State.IsPreparedFunc.Invoke();
        return true;
    }

    protected virtual bool _IsSucceed()
    {
        if (State.IsSucceedFunc != null)
            return State.IsSucceedFunc.Invoke();
        return false;
    }

    protected virtual bool _IsFailed()
    {
        if (State.IsFailedFunc != null)
            return State.IsFailedFunc.Invoke();
        return false;
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