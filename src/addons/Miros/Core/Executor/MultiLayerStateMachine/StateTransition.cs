using System;

namespace Miros.Core;

public class StateTransition
{
    public StateTransition(TaskBase toTask, Func<bool> condition = null,
        TransitionMode mode = TransitionMode.Normal)
    {
        ToTask = toTask;
        Condition = condition;
        Mode = mode;
    }

    public TaskBase ToTask { get; }
    public Func<bool> Condition { get; }
    public TransitionMode Mode { get; }

    public bool CanTransition()
    {
        if (Condition == null) return true;
        return Condition.Invoke();
    }
}