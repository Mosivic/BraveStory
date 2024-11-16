using System.Collections.Generic;

namespace Miros.Core;

public class ExecutorBase<TTask> : AbsExecutor<TTask>, IExecutor<TTask>
    where TTask : TaskBase
{
    protected Dictionary<Tag, TTask> _tasks = [];
    protected Persona _owner;

    public virtual void AddTask(TTask task)
    {
        _tasks[task.Sign] = task;
    }

    public virtual void RemoveTask(TTask task)
    {
        if (task.IsActive)
            task.Exit();
        _tasks.Remove(task.Sign);
    }

    public virtual double GetCurrentTaskTime(Tag layer)
    {
        return 0;
    }

    public virtual TTask GetLastTask(Tag layer)
    {
        return null;
    }

    public virtual TTask GetNowTask(Tag layer)
    {
        return null;
    }

    public virtual bool HasTaskRunning(TTask task)
    {
        return false;
    }

    public virtual void PhysicsUpdate(double delta)
    {
    }


    public virtual void Update(double delta)
    {
    }

    public virtual TTask[] GetAllTasks()
    {
        return [.. _tasks.Values];
    }
}