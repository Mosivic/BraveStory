namespace Miros.Core;

public class TaskBase(State state) : AbsTask(state), ITask
{
    public virtual void Enter()
    {
        State.Status = RunningStatus.Running;
        State.RunningTime = 0;

        OnEnter();
    }


    public virtual void Exit()
    {
        if (State.Status == RunningStatus.Succeed)
            Succeed();
        else if (State.Status == RunningStatus.Failed)
            Failed();

        OnExit();
    }

    
    public virtual bool CanEnter()
    {
        return OnEnterCondition();
    }


    // 状态退出条件,有三种情况：
    // 1. State.Status 为 RunningStatus.Succeed
    // 2. State.Status 为 RunningStatus.Failed
    // 3. State.ExitCondition 为真
    public virtual bool CanExit()
    {
        return State.Status == RunningStatus.Failed 
        || State.Status == RunningStatus.Succeed 
        || OnExitCondition() ;
    }


    public virtual bool CanRemove()
    {
        return OnRemoveCondition();
    }

    public virtual void TriggerOnAdd()
    {
        OnAdd();
    }

    public virtual void TriggerOnRemove()
    {
        OnRemove();
    }

    public virtual void Update(double delta)
    {
        if (State.Status != RunningStatus.Running) return;
        State.RunningTime += delta;
        OnUpdate(delta);
    }


    public virtual void PhysicsUpdate(double delta)
    {
        OnPhysicsUpdate(delta);
    }

    protected virtual void Succeed()
    {
        OnSucceed();
    }


    protected virtual void Failed()
    {
        OnFail();
    }
}