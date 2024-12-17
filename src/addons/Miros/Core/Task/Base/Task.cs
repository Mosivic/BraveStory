using Godot;

namespace Miros.Core;

public class Task<TState, THost, TContext, TExecuteArgs> : TaskBase<TState>
    where TState : State, new()
    where THost : Node
    where TContext : Context
    where TExecuteArgs : ExecuteArgs
{
    protected Agent Agent { get; private set; }
    public THost Host { get; private set; }
    public TContext Context { get; private set; }
    public virtual Tag StateTag { get; }
    public virtual TExecuteArgs ExecuteArgs { get; }


    public void Init(Agent agent, THost host, TContext context)
    {
        Agent = agent;
        Host = host;
        Context = context;

        InitState(StateTag, Agent);
    }
}