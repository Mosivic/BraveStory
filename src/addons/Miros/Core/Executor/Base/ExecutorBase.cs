namespace Miros.Core;

public class ExecutorBase<TTask> : AbsExecutor<TTask>, IExecutor
    where TTask : TaskBase
{
    public virtual void AddTask(ITask task)
    {
        _tasks.Add(task as TTask);
    }

    public virtual void RemoveTask(ITask task)
    {
        _tasks.Remove(task as TTask);
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
        return [.. _tasks];
    }
}