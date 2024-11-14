using System;

namespace Miros.Core;

public class StateTransition
{
    public StateTransition(Tag toJobSign, Func<bool> condition = null,
        StateTransitionMode mode = StateTransitionMode.Normal)
    {
        ToJobSign = toJobSign;
        Condition = condition;
        Mode = mode;
    }

    public Tag ToJobSign { get; }
    public Func<bool> Condition { get; }
    public StateTransitionMode Mode { get; }

    public bool CanTransition()
    {
        if (Condition == null) return true;
        return Condition.Invoke();
    }
}