using GPC.Job.Config;

namespace GPC;

public interface ICondition
{
    bool IsSatisfy();
    bool IsSatisfy(IState state);
}