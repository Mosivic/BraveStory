using GPC.Job.Config;

namespace GPC.AI.StateMachine;

public interface ITransition
{
    State To { get; }
    ICondition Condition { get; }
}