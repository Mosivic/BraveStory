using Godot;
using GPC.Job;
using GPC.State;

namespace GPC.Scheduler;


public interface IScheduler
{

}
public abstract class AbsScheduler : IScheduler
{
    protected JobWrapper JobWrapper = new();
    protected StateSpace StateSpace;

    public AbsScheduler(Node host,StateSpace stateSpace)
    {
        StateSpace = stateSpace;
        foreach (var state in StateSpace.States)
        {
            (state as IStateGeneric<Node>).Host = host;
            state.Scheduler = this;
        }
    }
    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);

}