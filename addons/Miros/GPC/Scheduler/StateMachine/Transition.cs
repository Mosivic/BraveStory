using GPC.State;

namespace GPC.Scheduler;

public class Transition<T> : ITransition<T> where T : IState
{
    public Transition(T to, Condition condition)
    {
        To = to;
        Condition = condition;
    }

    public T To { get; }
    public Condition Condition { get; }
}