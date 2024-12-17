namespace Miros.Core;


public class TaskBase<TState> : ITask<TState>
    where TState : State
{
    public virtual void Enter(TState state)
    {
        state.Status = RunningStatus.Running;
        state.RunningTime = 0;

        OnEnter(state);
    }


    public virtual void Exit(TState state)
    {
        if (state.Status == RunningStatus.Succeed)
            OnSucceed(state);
        else if (state.Status == RunningStatus.Failed)
            OnFailed(state);

        OnExit(state);
    }

    
    public virtual bool CanEnter(TState state)
    {
        return OnEnterCondition(state);
    }


    // 状态退出条件,有三种情况：
    // 1. State.Status 为 RunningStatus.Succeed
    // 2. State.Status 为 RunningStatus.Failed
    // 3. State.ExitCondition 为真
    public virtual bool CanExit(TState state)
    {
        return state.Status == RunningStatus.Failed 
        || state.Status == RunningStatus.Succeed 
        || OnExitCondition(state);
    }


    public virtual bool CanRemove(TState state)
    {
        return OnRemoveCondition(state);
    }

    public virtual void TriggerOnAdd(TState state)
    {
        OnAdd(state);
    }

    public virtual void TriggerOnRemove(TState state)
    {
        OnRemove(state);
    }

    public virtual void Update(TState state, double delta)
    {
        if (state.Status != RunningStatus.Running) return;
        state.RunningTime += delta;
        OnUpdate(state, delta);
    }


    public virtual void PhysicsUpdate(TState state, double delta)
    {
        if (state.Status != RunningStatus.Running) return;
        OnPhysicsUpdate(state, delta);
    }

    protected virtual void Succeed(TState state)
    {
        OnSucceed(state);
    }


    protected virtual void Failed(TState state)
    {
        OnFailed(state);
    }

    protected virtual void OnEnter(TState state)
    {
        state.EnterFunc?.Invoke();
    }

    protected virtual void OnExit(TState state)
    {
        state.ExitFunc?.Invoke();
    }

    protected virtual void OnUpdate(TState state, double delta)
    {
        state.UpdateFunc?.Invoke(delta);
    }

    protected virtual void OnPhysicsUpdate(TState state, double delta)
    {
        state.PhysicsUpdateFunc?.Invoke(delta);
    }

    protected virtual void OnSucceed(TState state)
    {
        state.SucceedFunc?.Invoke();
    }

    protected virtual void OnFailed(TState state)
    {
        state.FailedFunc?.Invoke();
    }

    protected virtual bool OnEnterCondition(TState state)
    {
        return state.EnterCondition?.Invoke() ?? true;
    }

    protected virtual bool OnExitCondition(TState state)
    {
        return state.ExitCondition?.Invoke() ?? true;
    }

    protected virtual bool OnRemoveCondition(TState state)
    {
        return state.RemoveCondition?.Invoke() ?? true;
    }

    protected virtual void OnAdd(TState state)
    {
        state.AddFunc?.Invoke();
    }

    protected virtual void OnRemove(TState state)
    {
        state.RemoveFunc?.Invoke();
    }

    public virtual void Stack(TState state, bool IsFromSameSource)
    {
        
    }
}