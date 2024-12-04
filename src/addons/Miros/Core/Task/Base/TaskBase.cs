namespace Miros.Core;

public class TaskBase(State state) : AbsTask(state), ITask
{
    private readonly State state = state;


    public virtual void Enter()
    {
        state.Status = RunningStatus.Running;
        state.RunningTime = 0;

        foreach (var component in state.Components.Values) component.Activate(this);

        OnEnter();
    }


    public virtual void Exit()
    {
        if (CanExit())
            Succeed();
        else
            Failed();

        foreach (var component in state.Components.Values) component.Deactivate(this);

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


    public T GetComponent<T>() where T : class
    {
        return state.Components.TryGetValue(typeof(T), out var component) ? component as T : null;
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
