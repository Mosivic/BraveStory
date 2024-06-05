using GPC.States;

namespace GPC.Scheduler;

public class Transition : ITransition
{
    public Transition(State to, Condition condition)
    {
        To = to;
        Condition = condition;
    }

    public State To { get; }
    public Condition Condition { get; }
}