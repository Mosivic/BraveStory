using System;

namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsTask(State state)
{
    public Tag Tag => state.Tag;
    public int Priority => state.Priority;
    public bool IsActive => state.IsActive;



    protected void OnEnter()
    {
        state.EnterFunc?.Invoke(state);
    }

    protected void OnExit()
    {
        state.ExitFunc?.Invoke(state);
    }

    protected void OnDeactivate()
    {
        state.PauseFunc?.Invoke(state);
    }

    protected void OnActivate()
    {
        state.ResumeFunc?.Invoke(state);
    }

    protected void OnUpdate(double delta)
    {
        state.UpdateFunc?.Invoke(state, delta);
    }

    protected void OnPhysicsUpdate(double delta)
    {
        state.PhysicsUpdateFunc?.Invoke(state, delta);
    }

    protected void OnSucceed()
    {
        state.SucceedFunc?.Invoke(state);
    }

    protected void OnFail()
    {
        state.FailedFunc?.Invoke(state);
    }

    protected bool OnCanEnter()
    {
        return state.EnterCondition?.Invoke(state) ?? true;
    }

    protected bool OnCanExit()
    {
        return state.ExitCondition?.Invoke(state) ?? true;
    }
}