namespace Miros.Core;

public class ExecutorBase<TTask> : AbsExecutor<TTask>, IExecutor
    where TTask : TaskBase
{
    public virtual void AddTask(ITask task, Context context)
    {
        var t = task as TTask;
        _tasks.Add(t.Tag, t);
    }

    public virtual void RemoveTask(ITask task)
    {
        var t = task as TTask;
        _tasks.Remove(t.Tag);
    }

    public virtual double GetCurrentTaskTime(Tag layer)
    {
        return 0;
    }

    public virtual ITask GetLastTask(Tag layer)
    {
        return null;
    }

    public virtual ITask GetNowTask(Tag layer)
    {
        return null;
    }

    public virtual bool HasTaskRunning(ITask task)
    {
        return false;
    }

    public virtual void PhysicsUpdate(double delta)
    {
    }


    public virtual void Update(double delta)
    {
    }

    public virtual ITask[] GetAllTasks()
    {
        return [.. _tasks.Values];
    }
}