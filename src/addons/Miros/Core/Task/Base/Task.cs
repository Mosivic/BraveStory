using Godot;
using Miros.Core;

namespace Miros.Core;

public class Task<TState, THost, TContext>() : TaskBase(new TState())
where TState : State, new()
where THost : Node
where TContext : Context
{
    protected Agent Agent { get; private set; }
    public THost Host { get; private set; }
    public TContext Context { get; private set; }
    public virtual Tag StateTag { get; }
    public virtual Tag LayerTag { get; } = Tags.Default;
    public virtual ExecutorType ExecutorType { get; }
    public virtual Transition[] Transitions { get; }
    

    public void Init(Agent agent, THost host, TContext context)
    {
        Agent = agent;
        Host = host;
        Context = context;

        InitState(StateTag, Agent);
    }
}