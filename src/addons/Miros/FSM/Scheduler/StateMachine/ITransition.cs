using System;
using FSM.States;

namespace FSM.Scheduler;

public interface ITransition
{
    AbsState To { get; }
    Func<bool> ConditionFunc { get; }
}