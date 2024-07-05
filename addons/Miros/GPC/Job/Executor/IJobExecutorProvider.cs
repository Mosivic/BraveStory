namespace GPC.Job.Executor;

public interface IJobExecutorProvider<T> where T : AbsJobExecutor,new()
{
    public static T Executor { get; set; }
}