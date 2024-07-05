using System;
using GPC.States;

namespace GPC.Scheduler;

public class Transition(CompoundState to, Func<bool> conditionFunc) : ITransition
{
    public AbsState To { get; } = to;
    public Func<bool> ConditionFunc { get; } = conditionFunc;
}