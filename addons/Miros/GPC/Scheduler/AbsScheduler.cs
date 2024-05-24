using System.Collections.Generic;
using GPC.Job;
using GPC.Job.Config;

namespace GPC.AI;

public abstract class AbsScheduler
{
    protected JobWrapper JobWrapper;
    protected List<State> States;

    public AbsScheduler(List<State> states)
    {
        States = states;
        JobWrapper = new JobWrapper();
    }

    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}