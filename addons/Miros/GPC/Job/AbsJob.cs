using GPC.Job.Config;

namespace GPC.Job;

public abstract class AbsJob
{
    protected virtual void _Enter(State state)
    {
        state.EnterFunc?.Invoke(state);
    }

    protected virtual void _Exit(State state)
    {
        state.ExitFunc?.Invoke(state);
    }

    protected virtual void _Pause(State state)
    {
        state.PauseFunc?.Invoke(state);
    }

    protected virtual void _Resume(State state)
    {
        state.ResumeFunc?.Invoke(state);
    }

    protected virtual bool _IsPrepared(State state)
    {
        if (state.IsPreparedFunc != null)
            return state.IsPreparedFunc.Invoke(state);
        if (state.Preconditions != null)
            return state.IsAllConditionSatisfy(state.Preconditions);
        return true;
    }

    protected virtual bool _IsSucceed(State state)
    {
        if (state.IsSucceedFunc != null)
            return state.IsSucceedFunc.Invoke(state);
        if (state.SuccessedConditions != null)
            return state.IsAllConditionSatisfy(state.SuccessedConditions);
        return false;
    }

    protected virtual bool _IsFailed(State state)
    {
        if (state.IsFailedFunc != null)
            return state.IsFailedFunc.Invoke(state);
        if (state.FailedConditions != null)
            return state.IsAnyConditionSatisfy(state.FailedConditions);
        return false;
    }

    protected virtual void _PhysicsUpdate(State state, double delta)
    {
        state.RunningPhysicsFunc?.Invoke(state);
    }

    protected virtual void _Update(State state, double delta)
    {
        state.RunningFunc?.Invoke(state);
    }

    protected virtual void _RunningInterval(State state)
    {
        state.RunningInterval?.Invoke(state);
    }
}