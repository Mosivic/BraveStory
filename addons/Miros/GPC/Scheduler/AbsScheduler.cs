using System.Collections.Generic;
using GPC.Job;
using GPC.Job.Config;

namespace GPC.AI;

public abstract class AbsScheduler<T>(StateSpace stateSpace) where T : IState
{
    protected JobWrapper<T> JobWrapper = new();
    protected StateSpace StateSpace = stateSpace;

    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}