﻿using System;

namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsTask(State state)
{
    public Tag Tag => state.Tag;
    public int Priority => state.Priority;
    public bool IsActive => state.IsActive;

    public event Action<State> OnEntered;
    public event Action<State> OnExited;
    public event Action<State, double> OnUpdated;
    public event Action<State, double> OnPhysicsUpdated;
    public event Action<State> OnSucceeded;
    public event Action<State> OnFailed;
    public event Action<State> OnPaused;
    public event Action<State> OnResumed;

    public event Func<State, bool> EnterCondition;
    public event Func<State, bool> ExitCondition;


    protected void OnEnter()
    {
        OnEntered?.Invoke(state);
    }

    protected void OnExit()
    {
        OnExited?.Invoke(state);
    }

    protected void OnDeactivate()
    {
        OnPaused?.Invoke(state);
    }

    protected void OnActivate()
    {
        OnResumed?.Invoke(state);
    }

    protected void OnUpdate(double delta)
    {
        OnUpdated?.Invoke(state, delta);
    }

    protected void OnPhysicsUpdate(double delta)
    {
        OnPhysicsUpdated?.Invoke(state, delta);
    }

    protected void OnSucceed()
    {
        OnSucceeded?.Invoke(state);
    }

    protected void OnFail()
    {
        OnFailed?.Invoke(state);
    }

    protected bool OnCanEnter()
    {
        return EnterCondition?.Invoke(state) ?? true;
    }

    protected bool OnCanExit()
    {
        return ExitCondition?.Invoke(state) ?? true;
    }
}