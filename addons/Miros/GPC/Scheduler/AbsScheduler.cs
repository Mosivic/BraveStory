using System;
using GPC.Job;
using GPC.Job.Executor;
using GPC.States;

namespace GPC.Scheduler;

public interface IScheduler
{
}

public abstract class AbsScheduler : IScheduler
{
    protected readonly StateSet StateSet;
    protected readonly AbsJobExecutor JobExecutor;
    
    protected AbsScheduler(StateSet stateSet)
    {
        StateSet = stateSet;
    }

    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);

    public void CreateJobByType(AbsState state)
    {
        
    }
}