namespace GPC.Job;

public abstract class AbsJob<T> where T : State.State
{
    protected virtual void _Enter(T state)
    {
        state.EnterFunc?.Invoke(state);
    }

    protected virtual void _Exit(T state)
    {
        state.ExitFunc?.Invoke(state);
    }

    protected virtual void _Pause(T state)
    {
        state.PauseFunc?.Invoke(state);
    }

    protected virtual void _Resume(T state)
    {
        state.ResumeFunc?.Invoke(state);
    }

    protected virtual bool _IsPrepared(T state)
    {
        if (state.IsPreparedFunc != null)
            return state.IsPreparedFunc.Invoke(state);
        if (state.PreCondition != null)
            return state.PreCondition.IsAllSatisfy(state);
        return true;
    }

    protected virtual bool _IsSucceed(T state)
    {
        if (state.IsSucceedFunc != null)
            return state.IsSucceedFunc.Invoke(state);
        if (state.SuccessedCondition != null)
            return state.SuccessedCondition.IsAllSatisfy(state);
        return false;
    }

    protected virtual bool _IsFailed(T state)
    {
        if (state.IsFailedFunc != null)
            return state.IsFailedFunc.Invoke(state);
        if (state.FailedCondition != null)
            return state.FailedCondition.IsAnySatisfy(state);
        return false;
    }

    protected virtual void _PhysicsUpdate(T state, double delta)
    {
        state.RunningPhysicsFunc?.Invoke(state);
    }

    protected virtual void _Update(T state, double delta)
    {
        state.RunningFunc?.Invoke(state);
    }

    protected virtual void _RunningInterval(T state)
    {
        state.RunningInterval?.Invoke(state);
    }
}