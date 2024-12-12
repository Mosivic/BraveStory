using System;

namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsTask(State state)
{
    public State State { get; private set; } = state;
    public void InitState(Tag tag,Agent source)
    {
        State.Tag = tag;
        State.Source = source;
    }
    public Tag Tag => State.Tag;
    public int Priority => State.Priority;
    public bool IsActive => State.IsActive;



    protected virtual void OnEnter()
    {
        OnEntered?.Invoke(State);
    }

    protected virtual void OnExit()
    {
        OnExited?.Invoke(State);
    }

    protected virtual void OnDeactivate()
    {
        OnPaused?.Invoke(State);
    }

    protected virtual void OnActivate()
    {
        OnResumed?.Invoke(State);
    }

    protected virtual void OnUpdate(double delta)
    {
        OnUpdated?.Invoke(State, delta);
    }

    protected virtual void OnPhysicsUpdate(double delta)
    {
        OnPhysicsUpdated?.Invoke(State, delta);
    }

    protected virtual void OnSucceed()
    {
        OnSucceeded?.Invoke(State);
    }

    protected virtual void OnFail()
    {
        OnFailed?.Invoke(State);
    }

    protected virtual bool OnEnterCondition()
    {
        return EnterCondition?.Invoke(State) ?? true;
    }

    protected virtual bool OnExitCondition()
    {
        return ExitCondition?.Invoke(State) ?? true;
    }
}