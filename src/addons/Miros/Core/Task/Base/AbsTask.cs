namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsTask(State state)
{
    public State State { get; } = state;
    public Tag Tag => State.Tag;
    public int Priority => State.Priority;
    public bool IsActive => State.IsActive;

    public void InitState(Tag tag, Agent source)
    {
        State.Tag = tag;
        State.Source = source;
    }


    protected virtual void OnEnter()
    {
        State.EnterFunc?.Invoke(State);
    }

    protected virtual void OnExit()
    {
        State.ExitFunc?.Invoke(State);
    }

    protected virtual void OnDeactivate()
    {
        State.PauseFunc?.Invoke(State);
    }

    protected virtual void OnActivate()
    {
        State.ResumeFunc?.Invoke(State);
    }

    protected virtual void OnUpdate(double delta)
    {
        State.UpdateFunc?.Invoke(State, delta);
    }

    protected virtual void OnPhysicsUpdate(double delta)
    {
        State.PhysicsUpdateFunc?.Invoke(State, delta);
    }

    protected virtual void OnSucceed()
    {
        State.SucceedFunc?.Invoke(State);
    }

    protected virtual void OnFail()
    {
        State.FailedFunc?.Invoke(State);
    }

    protected virtual bool OnEnterCondition()
    {
        return State.EnterCondition?.Invoke(State) ?? true;
    }

    protected virtual bool OnExitCondition()
    {
        return State.ExitCondition?.Invoke(State) ?? true;
    }
}