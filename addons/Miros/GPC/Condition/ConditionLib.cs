using GPC.Job.Config;

namespace GPC;

public class ConditionLib<T> where T : IState
{
    int CheckNum { get; set; }
}