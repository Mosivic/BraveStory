using Godot;
using GPC.Job;
using GPC.States;

namespace GPC.Scheduler;

public interface IScheduler
{
}

public abstract class AbsScheduler : IScheduler
{
    protected JobWrapper JobWrapper = new();
    protected StateSet StateSet;

    public AbsScheduler(StateSet stateSet)
    {
        StateSet = stateSet;
        foreach (var state in StateSet.States)
        {
            state.Scheduler = this;
        }
    }

    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}