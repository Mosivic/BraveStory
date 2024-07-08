using GPC.States;

namespace GPC.Job;

public abstract class JobBase(AbsState state) : AbsJob(state), IJob
{
    public virtual void Start()
    {
        State.Status = JobRunningStatus.Running;
        _Start();
    }

    public virtual void Succeed()
    {
        State.Status = JobRunningStatus.NoRun;
        _Succeed();
    }

    public virtual void Failed()
    {
        State.Status = JobRunningStatus.Failed;
        _Failed();
    }

    public virtual void Pause()
    {
        State.Status = JobRunningStatus.NoRun;
        _Pause();
    }

    public virtual void Resume()
    {
        State.Status = JobRunningStatus.Running;
        _Resume();
    }

    public virtual bool IsPrepared()
    {
        return _IsPrepared();
    }


    public virtual void Update(double delta)
    {
        if (_IsFailed())
            State.Status = JobRunningStatus.Failed;
        else if (_IsSucceed()) State.Status = JobRunningStatus.Succeed;

        State.IntervalElapsedTime += delta;
        if (State.IntervalElapsedTime > State.IntervalTime)
        {
            _IntervalUpdate();
            State.IntervalElapsedTime = 0;
        }

        _Update(delta);
    }

    public virtual void PhysicsUpdate(double delta)
    {
        _PhysicsUpdate(delta);
    }

    public virtual void IntervalUpdate()
    {
        _IntervalUpdate();
    }
}