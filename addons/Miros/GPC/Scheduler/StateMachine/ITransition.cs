using GPC.State;

namespace GPC.Scheduler;

public interface ITransition
{
    AbsState To { get; }
    Condition Condition { get; }
}