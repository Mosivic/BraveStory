
namespace Miros.Core;

public abstract class AbsJobBase(NativeState state) : AbsJob(state), IJob
{
    protected readonly NativeState state = state;

    NativeState IJob.State => state;

    public virtual void Enter()
    {
        state.Status = RunningStatus.Running;

        _Enter();
    }

    public virtual void Exit()
    {
        if(CanExit())
            Succeed();
        else
            Failed();

        _Exit();
    }


    public virtual void Pause()
    {
        state.Status = RunningStatus.NoRun;
        _Pause();
    }


    public virtual void Resume()
    {
        state.Status = RunningStatus.Running;
        _Resume();
    }


    public virtual bool CanEnter()
    {
        return _CanEnter();
    }


    public virtual bool CanExit()
    {
        return _CanExit();
    }
    

    public virtual void Update(double delta)
    {
        if (state.Status != RunningStatus.Running) return;
        _Update(delta);
    }

    public virtual void PhysicsUpdate(double delta)
    {
        _PhysicsUpdate(delta);
    }

    protected virtual void Succeed()
    {
        state.Status = RunningStatus.Succeed;
        _Succeed();
    }

    protected virtual void Failed()
    {
        state.Status = RunningStatus.Failed;
        _Failed();
    }
}