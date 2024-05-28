using GPC.Job.Config;

namespace GPC.AI.StateMachine;

public class Transition<T> : ITransition<T> where T : IState
{
    public Transition(T to, ICondition<T> condition)
    {
        To = to;
        Condition = condition;
    }

    public T To { get; }
    public ICondition<T> Condition { get; }
}