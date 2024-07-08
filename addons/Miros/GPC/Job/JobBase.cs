using GPC.States;

namespace GPC.Job;

public abstract class JobBase(AbsState state) : AbsJob(state), IJob
{
    public virtual void OnStart()
    {
        State.Status = JobRunningStatus.Running;
        _OnStart();
    }

    public virtual void OnSucceed()
    {
        _OnSucceed();
    }

    public virtual void OnFailed()
    {
        _OnFailed();
    }

    public virtual void OnPause()
    {
        State.Status = JobRunningStatus.NoRun;
        _OnPause();
    }

    public virtual void OnResume()
    {
        State.Status = JobRunningStatus.Running;
        _OnResume();
    }

    protected virtual void OnPeriod()
    {
        _OnPeriod();
    }

    public virtual bool IsPrepared()
    {
        return _IsPrepared();
    }

    protected virtual bool IsSucceed()
    {
        return _IsSucceed();
    }

    protected virtual bool IsFailed()
    {
        return _IsFailed();
    }
    
    
    public virtual void Update(double delta)
    {
        if (IsFailed())
            State.Status = JobRunningStatus.Failed;
        else if (IsSucceed()) 
            State.Status = JobRunningStatus.Succeed;

        State.DurationElapsed += delta;
        State.PeriodElapsed += delta;
        
        if (State.Duration > 0 && State.DurationElapsed > State.Duration)
        {
            State.Status = JobRunningStatus.Succeed;
            State.DurationElapsed = 0;
        }
        if (State.Period > 0 && State.PeriodElapsed > State.Period)
        {
            OnPeriod();
            State.PeriodElapsed = 0;
        }

        _Update(delta);
    }

    public virtual void PhysicsUpdate(double delta)
    {
        _PhysicsUpdate(delta);
    }
    
}