using GPC.State;

namespace GPC.Scheduler;

public interface ITransition<T> where T : IState
{
    T To { get; }
    Condition Condition { get; }
}