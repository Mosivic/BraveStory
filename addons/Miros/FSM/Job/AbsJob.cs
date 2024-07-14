﻿using FSM.States;

namespace FSM.Job;

public abstract class AbsJob(AbsState state)
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

    protected virtual void _OnSucceed()
    {
        state.OnSucceedFunc?.Invoke(state);
    }

    protected virtual void _OnFailed()
    {
        state.OnFailedFunc?.Invoke(state);
    }


    protected virtual void _OnPeriodOver()
    {
        state.OnPeriodOverFunc?.Invoke(state);
    }

    protected virtual void _OnStack()
    {
        state.OnStackFunc?.Invoke(state);
    }

    protected virtual void _OnStackOverflow()
    {
        state.OnStackOverflowFunc?.Invoke(state);
    }

    protected virtual void _OnDurationOVer()
    {
        state.OnDurationOverFunc?.Invoke(state);
    }


    protected virtual bool _IsSucceed()
    {
        var isSucceed = false;
        if (state.IsSucceedFunc != null)
            isSucceed = state.IsSucceedFunc.Invoke();

        return isSucceed;
    }

    protected virtual bool _IsFailed()
    {
        var isFailed = false;

        if (state.IsFailedFunc != null)
            isFailed = state.IsFailedFunc.Invoke();
        else if (state.UsePrepareFuncAsRunCondition && state.IsPreparedFunc != null)
            isFailed = !state.IsPreparedFunc.Invoke();

        return isFailed;
    }

    protected virtual void _PhysicsUpdate(double delta)
    {
        state.PhysicsUpdateFunc?.Invoke(state,delta);
    }

    protected virtual void _Update(double delta)
    {
        state.UpdateFunc?.Invoke(state,delta);
    }
}