namespace Miros.Core;

public abstract class JobBase : AbsJob, IJob
{
    protected JobBase(AbsState state) : base(state)
    {
        this.state = state;
    }

    private readonly AbsState state;
    
    public virtual void Enter()
    {
        state.Status = RunningStatus.Running;
        
        foreach (var component in state.Components.Values)
        {
            component.Activate(this);
        }

        OnEnter();
    }


    public virtual void Exit()
    {
        if(CanExit())
            Succeed();
        else
            Failed();

        foreach (var component in state.Components.Values)
        {
            component.Deactivate(this);
        }    

        OnExit();
    }


    public virtual void Pause()
    {
        state.Status = RunningStatus.NoRun;
        OnPause();
    }


    public virtual void Resume()
    {
        state.Status = RunningStatus.Running;
        OnResume();
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