using System;

namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsJob(AbsState state)
{

    public string Name => state.Name;
    public string Description => state.Description;
    public int Priority => state.Priority;
    public Tag Layer => state.Layer;
    public RunningStatus Status => state.Status;



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


    protected void OnEnter()
    {
        OnEntered?.Invoke(state);
    }

    protected void OnExit()
    {
        OnExited?.Invoke(state);
    }

    protected void OnPause()
    {
        OnPaused?.Invoke(state);
    }

    protected void OnResume()
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