using GPC.State;

namespace GPC.Job;

public abstract class AbsJob
{
    protected virtual void _Enter(AbsState state)
    {
        state.EnterFunc?.Invoke(state);
    }

    protected virtual void _Exit(AbsState state)
    {
        state.ExitFunc?.Invoke(state);
    }

    protected virtual void _Pause(AbsState state)
    {
        state.PauseFunc?.Invoke(state);
    }

    protected virtual void _Resume(AbsState state)
    {
        state.ResumeFunc?.Invoke(state);
    }

    protected virtual bool _IsPrepared(AbsState state)
    {
        if (state.IsPreparedFunc != null)
            return state.IsPreparedFunc.Invoke(state);
        if (state.PreCondition != null)
            return state.PreCondition.IsAllSatisfy(state);
        return true;
    }

    protected virtual bool _IsSucceed(AbsState state)
    {
        if (state.IsSucceedFunc != null)
            return state.IsSucceedFunc.Invoke(state);
        if (state.SuccessedCondition != null)
            return state.SuccessedCondition.IsAllSatisfy(state);
        return false;
    }

    protected virtual bool _IsFailed(AbsState state)
    {
        if (state.IsFailedFunc != null)
            return state.IsFailedFunc.Invoke(state);
        if (state.FailedCondition != null)
            return state.FailedCondition.IsAnySatisfy(state);
        return false;
    }

    protected virtual void _PhysicsUpdate(AbsState state, double delta)
    {
        state.RunningPhysicsFunc?.Invoke(state);
    }

    protected virtual void _Update(AbsState state, double delta)
    {
        state.RunningFunc?.Invoke(state);
    }

    protected virtual void _RunningInterval(AbsState state)
    {
        state.RunningInterval?.Invoke(state);
    }
}