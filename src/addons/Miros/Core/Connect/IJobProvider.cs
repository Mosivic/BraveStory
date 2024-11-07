namespace Miros.Core;

public interface IJobProvider
{
    IJob GetJob(AbsState state);
    IJob[] GetAllJobs();
}