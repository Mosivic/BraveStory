namespace GPC.Job.Executor;

public interface IJobExecutorProvider<out T> where T : AbsJobExecutor, new()
{
    T Executor { get; }
}