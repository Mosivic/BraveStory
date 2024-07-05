using System;
using GPC.Job;
using GPC.Job.Executor;
using GPC.States;

namespace GPC.Scheduler;

public abstract class AbsScheduler(StateSet stateSet) 
{
    protected readonly StateSet StateSet = stateSet;
    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}