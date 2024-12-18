using System;


namespace Miros.Core;

public class ActionState : State
{
    public virtual Tag Layer { get; set; } 
    public virtual Transition[] Transitions { get; set; }
    public virtual bool AsDefaultTask { get; set; } = false;
    public virtual bool AsNextTask { get; set; } = false;
    public virtual TransitionMode AsNextTaskTransitionMode { get; set; }
}  
