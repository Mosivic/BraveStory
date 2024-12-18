using System;
using Godot;

namespace Miros.Core;

public class ActionState<THost, TContext> : State
    where THost : Node
    where TContext : Context
{
    public THost Host { get; private set; }
    public TContext Context { get; private set; }

    public virtual Tag Layer { get; set; } 
    public virtual Transition[] Transitions { get; set; }
    public virtual bool AsDefaultTask { get; set; } = false;
    public virtual bool AsNextTask { get; set; } = false;
    public virtual TransitionMode AsNextTaskTransitionMode { get; set; }
    


    public virtual void Init(THost host, TContext context)
    {
        Host = host;
        Context = context;
    }
}