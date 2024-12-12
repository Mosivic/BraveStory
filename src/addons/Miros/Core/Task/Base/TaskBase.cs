namespace Miros.Core;

public class TaskBase : AbsTask, ITask
{

      public virtual void Enter()
    {
        state.Status = RunningStatus.Running;
        state.RunningTime = 0;

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
        state.Status = RunningStatus.NoRun;
        OnDeactivate();
    }


    public virtual void Activate()
    {
        state.Status = RunningStatus.Running;
        OnActivate();
    }


    public virtual bool CanEnter()
    {
        return OnCanEnter();
    }


    public virtual bool CanExit()
    {
        return OnCanExit();
    }

    public virtual void Update(double delta)
    {
        if (state.Status != RunningStatus.Running) return;
        state.RunningTime += delta;
        OnUpdate(delta);
    }


    public virtual void PhysicsUpdate(double delta)
    {
        OnPhysicsUpdate(delta);
    }

    protected virtual void Succeed()
    {
        state.Status = RunningStatus.Succeed;
        OnSucceed();
    }


    protected virtual void Failed()
    {
        state.Status = RunningStatus.Failed;
        OnFail();
    }
}