using Miros.Core;

namespace BraveStory;

public class Task<TState, THost, TContext> : TaskBase
where TState : State, new()
where THost : Character
where TContext : Context
{
    protected Agent Agent { get; private set; }
    public THost Host { get; private set; }
    public TContext Context { get; private set; }
    public virtual Tag StateTag { get; }
    public virtual Tag LayerTag { get; } = Tags.Default;
    public virtual ExecutorType ExecutorType { get; }
    public virtual Transition[] Transitions { get; }

    public Task(Agent agent, THost host, TContext context)
    {
        Agent = agent;
        Host = host;
        Context = context;
        SetState(new TState() { Tag = StateTag, Source = Agent});
    }
}
