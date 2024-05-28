using GPC.Job.Config;

namespace GPC.AI.StateMachine;

public class Transition<T> : ITransition<T> where T : IState
{
    public Transition(T to, Condition<T> condition)
    {
        To = to;
        Condition = condition;
    }

    public T To { get; }
    public Condition<T> Condition { get; }
}