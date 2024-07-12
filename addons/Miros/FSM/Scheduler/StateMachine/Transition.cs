using System;
using FSM.States;

namespace FSM.Scheduler;

public class Transition(CompoundState to, Func<bool> conditionFunc) : ITransition
{
    public AbsState To { get; } = to;
    public Func<bool> ConditionFunc { get; } = conditionFunc;
}