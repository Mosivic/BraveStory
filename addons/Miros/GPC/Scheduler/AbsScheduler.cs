using Godot;
using GPC.Job;
using GPC.State;

namespace GPC.Scheduler;


public interface IScheduler<out T> 
{

}
public abstract class AbsScheduler<T> : IScheduler<IState> where T : IState
{
    protected JobWrapper<T> JobWrapper = new();
    protected StateSpace StateSpace;

    public AbsScheduler( Node host,StateSpace stateSpace)
    {
        StateSpace = stateSpace;
        foreach (var state in StateSpace.States)
        {
            state.Host = host;
            state.Scheduler = this;
        }
    }
    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);

}