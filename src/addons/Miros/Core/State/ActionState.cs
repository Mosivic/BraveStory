using System;


namespace Miros.Core;

public class ActionState<TContext> : State
    where TContext : Context
{
    protected TContext Context { get; private set; }

    public virtual Tag Layer { get; set; } 
    public virtual Transition[] Transitions { get; set; }
    public virtual bool AsDefaultTask { get; set; } = false;
    public virtual bool AsNextTask { get; set; } = false;
    public virtual TransitionMode AsNextTaskTransitionMode { get; set; }
    


    public virtual void Init(TContext context)
    {
        Context = context;
    }
}