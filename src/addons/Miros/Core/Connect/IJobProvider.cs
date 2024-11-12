namespace Miros.Core;

public interface IJobProvider
{
    JobBase GetJob(AbsState state);
    JobBase[] GetAllJobs();
}
