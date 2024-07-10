using GPC.States;

namespace GPC.Job.Executor;

public interface IJobProvider
{
    IJob GetJob(AbsState state);
}