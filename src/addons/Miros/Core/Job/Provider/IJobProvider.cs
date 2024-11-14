namespace Miros.Core;

public interface IJobProvider
{
    JobBase GetJob(State state);
    JobBase[] GetAllJobs();
}