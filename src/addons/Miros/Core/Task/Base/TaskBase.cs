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
        if (CanExit())
            Succeed();
        else
            Failed();

        OnExit();
    }


    public virtual void Deactivate()
    {
        State.Status = RunningStatus.NoRun;
        OnDeactivate();
    }


    public virtual void Activate()
    {
        State.Status = RunningStatus.Running;
        OnActivate();
    }


    public virtual bool CanEnter()
    {
        return OnEnterCondition();
    }


    public virtual bool CanExit()
    {
        return OnExitCondition();
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
        State.Status = RunningStatus.Succeed;
        OnSucceed();
    }


    protected virtual void Failed()
    {
        State.Status = RunningStatus.Failed;
        OnFail();
    }
}