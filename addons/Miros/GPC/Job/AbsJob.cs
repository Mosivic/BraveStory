using GPC.States;

namespace GPC.Job;

public abstract class AbsJob(State state)
{
    protected virtual void _Enter()
    {
        state.EnterFunc?.Invoke(state);
    }

    protected virtual void _Exit()
    {
        state.ExitFunc?.Invoke(state);
    }

    protected virtual void _Pause()
    {
        state.PauseFunc?.Invoke(state);
    }

    protected virtual void _Resume()
    {
        state.ResumeFunc?.Invoke(state);
    }

    protected virtual bool _IsPrepared()
    {
        if (state.IsPreparedFunc != null)
            return state.IsPreparedFunc.Invoke();
        return true;
    }

    protected virtual bool _IsSucceed()
    {
        if (state.IsSucceedFunc != null)
            return state.IsSucceedFunc.Invoke();
        return false;
    }

    protected virtual bool _IsFailed()
    {
        if (state.IsFailedFunc != null)
            return state.IsFailedFunc.Invoke();
        return false;
    }

    protected virtual void _PhysicsUpdate(double delta)
    {
        state.RunningPhysicsFunc?.Invoke(state);
    }

    protected virtual void _Update(double delta)
    {
        state.RunningFunc?.Invoke(state);
    }

    protected virtual void _RunningInterval()
    {
        state.RunningInterval?.Invoke(state);
    }
}