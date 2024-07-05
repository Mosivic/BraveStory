using System;
using GPC.States;

namespace GPC.Scheduler;

public class Transition : ITransition
{
    public Transition(CompoundState to, Func<bool> conditionFunc)
    {
        To = to;
        ConditionFunc = conditionFunc;
    }

    public AbsState To { get; }
    public Func<bool> ConditionFunc { get; }
}