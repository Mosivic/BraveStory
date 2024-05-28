using GPC.Job.Config;

namespace GPC;

public interface ICondition<T> where T : IState
{
    int CheckNum { get; set; }
    bool IsSatisfy();
    bool IsSatisfy(T state);
}