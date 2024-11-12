﻿using System;

namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsJob(AbsState state)
{
    public event Action<AbsState> OnEntered;
    public event Action<AbsState> OnExited;
    public event Action<AbsState, double> OnUpdated;
    public event Action<AbsState, double> OnPhysicsUpdated;
    public event Action<AbsState> OnSucceeded;
    public event Action<AbsState> OnFailed;
    public event Action<AbsState> OnPaused;
    public event Action<AbsState> OnResumed;

    public event Func<AbsState, bool> EnterCondition;
    public event Func<AbsState, bool> ExitCondition;




    protected virtual void OnEnter()
    {
        OnEntered?.Invoke(state); 
    }

    protected virtual void OnExit()
    {
        OnExited?.Invoke(state);
    }

    protected virtual void OnPause()
    {
        OnPaused?.Invoke(state);
    }

    protected virtual void OnResume()
    {
        OnResumed?.Invoke(state);
    }       

    protected virtual void OnUpdate(double delta)
    {
        OnUpdated?.Invoke(state, delta);
    }   

    protected virtual void OnPhysicsUpdate(double delta)
    {
        OnPhysicsUpdated?.Invoke(state, delta);
    }

    protected virtual void OnSucceed()
    {
        OnSucceeded?.Invoke(state );
    }

    protected virtual void OnFail()
    {
        OnFailed?.Invoke(state);
    }

    protected virtual bool OnCanEnter()
    {
        return EnterCondition?.Invoke(state) ?? true;
    }

    protected virtual bool OnCanExit()
    {
        return ExitCondition?.Invoke(state) ?? true;
    }
}   