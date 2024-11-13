using System;
using System.Collections.Generic;

namespace Miros.Core;
public class StateTransition
{
    public Tag ToJobSign { get; }
    public Func<bool> Condition { get; }
    public StateTransitionMode Mode { get; }
    public StateTransition(Tag toJobSign,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        ToJobSign = toJobSign;
        Condition = condition;
        Mode = mode;
    }
    
    public bool CanTransition()
    {
        if(Condition == null) return true;
        return Condition.Invoke();
    }
} 