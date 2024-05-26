using GPC.Job.Config;

namespace GPC;

public interface ICondition
{
    bool IsMark { get; set; }
    bool IsSatisfy();
    bool IsSatisfy(IState state);
}