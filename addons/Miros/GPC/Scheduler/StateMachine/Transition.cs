using GPC.Job.Config;

namespace GPC.AI.StateMachine;

public class Transition : ITransition
{
    public Transition(State to, ICondition condition)
    {
        To = to;
        Condition = condition;
    }

    public State To { get; }
    public ICondition Condition { get; }
}