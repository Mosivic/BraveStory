using System;
using System.Collections.Generic;

namespace Miros.Core;
public class StateTransition
{
    public NativeJob ToJob { get; }
    public Func<bool> Condition { get; }
    public StateTransitionMode Mode { get; }
    public StateTransition(NativeJob toJob,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        ToJob = toJob;
        Condition = condition;
        Mode = mode;
    }
    
    public bool CanTransition()
    {
        if(Condition == null) return true;
        return Condition.Invoke();
    }
} 