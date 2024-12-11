using System;

namespace Miros.Core;

public struct Transition
{
    public Tag To { get; init; }
    public Func<bool> Condition { get; }
    public TransitionMode Mode { get; }

    public Transition(Tag to, Func<bool> condition = null, TransitionMode mode = TransitionMode.Normal)
    {
        To = to;
        Condition = condition;
        Mode = mode;
    }
}
