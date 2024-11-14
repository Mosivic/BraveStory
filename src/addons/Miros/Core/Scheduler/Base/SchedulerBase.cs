using System.Collections.Generic;

namespace Miros.Core;

public class SchedulerBase<TJob> : AbsScheduler<TJob>, IScheduler<TJob>
    where TJob : JobBase
{
    protected Dictionary<Tag, TJob> _jobs = [];
    protected Persona _owner;

    public virtual void AddJob(TJob job)
    {
        _jobs[job.Sign] = job;
    }

    public virtual void RemoveJob(TJob job)
    {
        if (job.IsActive)
            job.Exit();
        _jobs.Remove(job.Sign);
    }

    public virtual double GetCurrentJobTime(Tag layer)
    {
        return 0;
    }

    public virtual TJob GetLastJob(Tag layer)
    {
        return null;
    }

    public virtual TJob GetNowJob(Tag layer)
    {
        return null;
    }

    public virtual bool HasJobRunning(TJob job)
    {
        return false;
    }

    public virtual void PhysicsUpdate(double delta)
    {
    }


    public virtual void Update(double delta)
    {
    }

    public virtual TJob[] GetAllJobs()
    {
        return [.. _jobs.Values];
    }
}