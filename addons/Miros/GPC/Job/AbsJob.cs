﻿using GPC.Job.Config;

namespace GPC.Job;

public abstract class AbsJob<T> where T : State
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
        if (state.Preconditions != null)
            return state.IsAllConditionSatisfy(state.Preconditions);
        return true;
    }

    protected virtual bool _IsSucceed(T state)
    {
        if (state.IsSucceedFunc != null)
            return state.IsSucceedFunc.Invoke(state);
        if (state.SuccessedConditions != null)
            return state.IsAllConditionSatisfy(state.SuccessedConditions);
        return false;
    }

    protected virtual bool _IsFailed(T state)
    {
        if (state.IsFailedFunc != null)
            return state.IsFailedFunc.Invoke(state);
        if (state.FailedConditions != null)
            return state.IsAnyConditionSatisfy(state.FailedConditions);
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