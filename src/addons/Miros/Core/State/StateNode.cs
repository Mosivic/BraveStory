using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class Shared{}


[GodotClassName("StateNode")]
public abstract partial class StateNode<TState, THost,TShared> : Node 
where THost : Node 
where TState : State, new()
where TShared : Shared, new()
{
    protected Agent Agent { get; private set; }
    protected THost Host { get; private set; }
    public TState State { get; private set; }
    public abstract Tag StateTag { get; }
    public abstract Tag LayerTag { get; }
    public abstract ExecutorType ExecutorType { get; }
    public virtual Transition[] Transitions { get; }
    public virtual Transition AnyTransition { get; }
    public TShared Shared { get; set; }


    public void Initialize(Agent agent, THost host,TShared shared)
    {
        Agent = agent;
        Host = host;
        Shared = shared;

        State = new TState
        {
            Tag = StateTag,
            Source = Agent,
        };
        
        State.OnEntered(State => Enter());
        State.OnExited(State => Exit());
        State.OnUpdated((State, delta) => Update(delta));
        State.OnPhysicsUpdated((State, delta) => PhysicsUpdate(delta));
    }
    
    protected virtual void Enter() { }

    protected virtual void Exit() { }

    protected virtual void Update(double delta) { }

    protected virtual void PhysicsUpdate(double delta) { }

    protected virtual bool EnterCondition() { return true;}

    protected virtual bool ExitCondition() { return true;}
}