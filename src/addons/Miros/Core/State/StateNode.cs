using System.Collections.Generic;
using Godot;

namespace Miros.Core;


public abstract partial class StateNode<THost> : Node where THost : Node
{
    protected Agent Agent { get; private set; }
    protected THost Host { get; private set; }
    protected abstract Tag StateTag { get; init; }
    protected virtual Transition[] Transitions { get; init; }
    protected virtual bool IsAnyTransState { get; init; } = false;
    protected Dictionary<string, dynamic> Res { get; private set; }


    public void Initialize(Agent agent, THost host)
    {
        Agent = agent;
        Host = host;

        var State = new State(StateTag, Agent);

        State.OnEntered(State => Enter());
        State.OnExited(State => Exit());
        State.OnUpdated((State, delta) => Update(delta));
        State.OnPhysicsUpdated((State, delta) => PhysicsUpdate(delta));

        ShareRes();
        
    }

    protected virtual void ShareRes(){ }
    
    protected virtual void Enter() { }

    protected virtual void Exit() { }

    protected virtual void Update(double delta) { }

    protected virtual void PhysicsUpdate(double delta) { }

    protected virtual bool EnterCondition() { return true;}

    protected virtual bool ExitCondition() { return true;}
}