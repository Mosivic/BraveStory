using System;
using GPC.States;

namespace GPC.Scheduler;

public interface ITransition
{
    AbsState To { get; }
    Func<bool> ConditionFunc { get; }
}