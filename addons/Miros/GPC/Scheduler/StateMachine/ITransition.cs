using GPC.States;

namespace GPC.Scheduler;

public interface ITransition
{
    State To { get; }
    Condition Condition { get; }
}