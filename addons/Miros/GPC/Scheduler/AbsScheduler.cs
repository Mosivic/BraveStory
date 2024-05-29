using GPC.Job;
using GPC.State;

namespace GPC.Scheduler;

public abstract class AbsScheduler<T>(StateSpace stateSpace) where T : IState
{
    protected JobWrapper<T> JobWrapper = new();
    protected StateSpace StateSpace = stateSpace;

    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}