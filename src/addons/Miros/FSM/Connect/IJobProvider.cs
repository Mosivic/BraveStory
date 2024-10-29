using FSM.States;

namespace FSM.Job.Executor;

public interface IJobProvider
{
    IJob GetJob(AbsState state);
    IJob[] GetAllJobs();
}