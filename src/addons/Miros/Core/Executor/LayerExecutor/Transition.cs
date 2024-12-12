using System;

namespace Miros.Core;

public struct Transition
{
    public Tag To { get; init; } = Tags.Default;
    public Func<bool> Condition { get; }
    public TransitionMode Mode { get; }
    public bool IsAny { get; } = false;
    public int Priority { get; }

    public Transition(Tag to, Func<bool> condition = null, TransitionMode mode = TransitionMode.Normal,
        int priority = 0, bool isAny = false)
    {
        To = to;
        Condition = condition;
        Mode = mode;
        Priority = priority;
        IsAny = isAny;
    }

    public readonly bool CanTransition()
    {
        if (Condition == null) return true;
        return Condition.Invoke();
    }
}