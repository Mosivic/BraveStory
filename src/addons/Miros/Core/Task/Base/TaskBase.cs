namespace Miros.Core;


public class TaskBase<TState> : ITask
    where TState : State
{
    public TState State { get;set; }

    public virtual void Enter(State state)
    {
        state.Status = RunningStatus.Running;
        state.RunningTime = 0;

        OnEnter(state);
    }


    public virtual void Exit(State state)
    {
        if (state.Status == RunningStatus.Succeed)
            OnSucceed(state);
        else if (state.Status == RunningStatus.Failed)
            OnFailed(state);

        OnExit(state);
    }

    
    public virtual bool CanEnter(State state)
    {
        return OnEnterCondition(state);
    }


    // 状态退出条件,有三种情况：
    // 1. State.Status 为 RunningStatus.Succeed
    // 2. State.Status 为 RunningStatus.Failed
    // 3. State.ExitCondition 为真
    public virtual bool CanExit(State state)
    {
        return state.Status == RunningStatus.Failed 
        || state.Status == RunningStatus.Succeed 
        || OnExitCondition(state);
    }


    public virtual bool CanRemove(State state)
    {
        return OnRemoveCondition(state);
    }

    public virtual void TriggerOnAdd(State state)
    {
        OnAdd(state);
    }

    public virtual void TriggerOnRemove(State state)
    {
        OnRemove(state);
    }

    public virtual void Update(State state, double delta)
    {
        if (state.Status != RunningStatus.Running) return;
        state.RunningTime += delta;
        OnUpdate(state, delta);
    }


    public virtual void PhysicsUpdate(State state, double delta)
    {
        if (state.Status != RunningStatus.Running) return;
        OnPhysicsUpdate(state, delta);
    }

    protected virtual void Succeed(State state)
    {
        OnSucceed(state);
    }


    protected virtual void Failed(State state)
    {
        OnFailed(state);
    }

    protected virtual void OnEnter(State state)
    {
        state.EnterFunc?.Invoke();
    }

    protected virtual void OnExit(State state)
    {
        state.ExitFunc?.Invoke();
    }

    protected virtual void OnUpdate(State state, double delta)
    {
        state.UpdateFunc?.Invoke(delta);
    }

    protected virtual void OnPhysicsUpdate(State state, double delta)
    {
        state.PhysicsUpdateFunc?.Invoke(delta);
    }

    protected virtual void OnSucceed(State state)
    {
        state.SucceedFunc?.Invoke();
    }

    protected virtual void OnFailed(State state)
    {
        state.FailedFunc?.Invoke();
    }

    protected virtual bool OnEnterCondition(State state)
    {
        return state.EnterCondition?.Invoke() ?? true;
    }

    protected virtual bool OnExitCondition(State state)
    {
        return state.ExitCondition?.Invoke() ?? true;
    }

    protected virtual bool OnRemoveCondition(State state)
    {
        return state.RemoveCondition?.Invoke() ?? true;
    }

    protected virtual void OnAdd(State state)
    {
        state.AddFunc?.Invoke();
    }

    protected virtual void OnRemove(State state)
    {
        state.RemoveFunc?.Invoke();
    }

    public virtual void Stack(State state, bool IsFromSameSource)
    {
        
    }
}