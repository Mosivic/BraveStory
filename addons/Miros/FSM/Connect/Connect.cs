using System.Collections.Generic;
using System.Linq;
using FSM.Scheduler;
using FSM.States;

namespace FSM.Job.Executor;

public class Connect<TJobProvider, TScheduler> : IConnect
    where TJobProvider : IJobProvider, new()
    where TScheduler : IScheduler, new()
{
    public Connect(List<AbsState> states)
    {
        JobProvider = new TJobProvider();
        Scheduler = new TScheduler();

        foreach (var job in states.Select(state => JobProvider.GetJob(state)))
            Scheduler.AddJob(job);
    }

    private TJobProvider JobProvider { get; }
    private TScheduler Scheduler { get; }


    public void AddState(AbsState state)
    {
        var job = JobProvider.GetJob(state);
        Scheduler.AddJob(job);
    }


    public void RemoveState(AbsState state)
    {
        var job = JobProvider.GetJob(state);
        Scheduler.RemoveJob(job);
    }

    public void Update(double delta)
    {
        Scheduler.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        Scheduler.PhysicsUpdate(delta);
    }

    public bool HasStateRunning(AbsState state)
    {
        var job = JobProvider.GetJob(state);
        return HasJobRunning(job);
    }

    public bool HasAnyStateRunning(AbsState[] states)
    {
        return states.Select(state => JobProvider.GetJob(state)).Any(job => Scheduler.HasJobRunning(job));
    }


    public bool HasAllStateRunning(AbsState[] states)
    {
        return states.Select(state => JobProvider.GetJob(state)).All(job => Scheduler.HasJobRunning(job));
    }

    public IJob[] GetAllJobs()
    {
        return JobProvider.GetAllJobs();
    }

    public bool HasJobRunning(IJob job)
    {
        return Scheduler.HasJobRunning(job);
    }
}