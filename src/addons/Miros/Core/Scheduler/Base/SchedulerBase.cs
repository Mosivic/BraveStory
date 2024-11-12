using System.Collections.Generic;

namespace Miros.Core;

public class SchedulerBase<TJob> : AbsScheduler<TJob>, IScheduler<TJob>
    where TJob : JobBase
{
    protected List<TJob> _jobs = [];

    public virtual void AddJob(TJob job)
    {
        _jobs.Add(job);
    }

    public virtual void RemoveJob(TJob job)
    {
        if (job.Status == RunningStatus.Running)
            job.Exit();
        _jobs.Remove(job);
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
        return;
    }


    public virtual void Update(double delta)
    {
        return;
    }

    public virtual TJob[] GetAllJobs()
    {
        return [.. _jobs];
    }
}