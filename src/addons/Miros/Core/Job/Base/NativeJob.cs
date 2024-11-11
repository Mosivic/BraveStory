
namespace Miros.Core;

public abstract class NativeJob(NativeState state) : AbsJob(state), IJob
{
    protected readonly NativeState state = state;

    NativeState IJob.State => state;

    /// <summary>
    /// 注册组件的事件处理器
    /// </summary>
    protected virtual void RegisterHandlers()
    {
        foreach (var component in state.Components.Values)
        {
            component.RegisterHandler(this);
        }
    }

    /// <summary>
    /// 注销组件的事件处理器
    /// </summary>
    protected virtual void UnregisterHandlers()
    {
        foreach (var component in state.Components.Values)
        {
            component.UnregisterHandler(this);
        }
    }


    public virtual void Enter()
    {
        state.Status = RunningStatus.Running;

        OnEnter();
    }

    public virtual void Exit()
    {
        if(CanExit())
            Succeed();
        else
            Failed();

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