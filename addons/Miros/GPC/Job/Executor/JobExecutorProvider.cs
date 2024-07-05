namespace GPC.Job.Executor;

public class JobExecutorProvider<T>:IJobExecutorProvider<T> where T : AbsJobExecutor,new()
{
    private static T _executor;
    public T Executor => GetIns();
    
    private static T GetIns()
    {
        return _executor ??= new T();
    }
}