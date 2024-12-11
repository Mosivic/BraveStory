using System;

namespace Miros.Core;

public struct Transition
{
    public Tag To { get; init; }
    public Func<bool> Condition { get; }
    public StateTransitionMode Mode { get; }

    public Transition(Tag to, Func<bool> condition, StateTransitionMode mode = StateTransitionMode.Normal)
    {
        To = to;
        Condition = condition;
        Mode = mode;
    }
}
