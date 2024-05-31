using GPC.State;

namespace GPC.Scheduler;

public class Transition: ITransition
{
    public Transition(AbsState to, Condition condition)
    {
        To = to;
        Condition = condition;
    }

    public AbsState To { get; }
    public Condition Condition { get; }
}