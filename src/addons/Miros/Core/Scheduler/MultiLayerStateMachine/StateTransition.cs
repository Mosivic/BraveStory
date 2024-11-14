using System;

namespace Miros.Core;

public class StateTransition
{
    public StateTransition(JobBase toJob, Func<bool> condition = null,
        StateTransitionMode mode = StateTransitionMode.Normal)
    {
        ToJob = toJob;
        Condition = condition;
        Mode = mode;
    }

    public JobBase ToJob { get; }
    public Func<bool> Condition { get; }
    public StateTransitionMode Mode { get; }

    public bool CanTransition()
    {
        if (Condition == null) return true;
        return Condition.Invoke();
    }
}