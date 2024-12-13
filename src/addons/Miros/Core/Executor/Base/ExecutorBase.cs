namespace Miros.Core;

public class ExecutorBase<TTask> : AbsExecutor<TTask>, IExecutor
    where TTask : TaskBase
{
    public virtual void AddTask(ITask task, Context context)
    {
        var t = task as TTask;
        if(t.RemovePolicy != RemovePolicy.None)
            _tempTasks.Add(t.Tag, t);
        _tasks.Add(t.Tag, t);
    }

    public virtual void RemoveTask(ITask task)
    {
        var t = task as TTask;
        if (t.RemovePolicy != RemovePolicy.None)
            _tempTasks.Remove(t.Tag);
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
        UpdateTempTasks();
    }

    public virtual ITask[] GetAllTasks()
    {
        return [.. _tasks.Values];
    }

    private void UpdateTempTasks()
    {
        foreach (var task in _tempTasks.Values)
        {
            RemoveTempTask(task);
        }
    }

    private void RemoveTempTask(TTask task)
    {
        var removePolicy = task.RemovePolicy;
        switch (removePolicy)
        {
            case RemovePolicy.Condition:
                if (task.CanRemove())
                    RemoveTask(task);
                break;
            case RemovePolicy.WhenFailed:
                if (task.Status() == RunningStatus.Failed)
                    RemoveTask(task);
                break;
            case RemovePolicy.WhenSucceed:
                if (task.Status() == RunningStatus.Succeed)
                    RemoveTask(task);
                break;
            case RemovePolicy.WhenExited:
                if (task.Status() == RunningStatus.Succeed 
                || task.Status() == RunningStatus.Failed)
                    RemoveTask(task);
                break;
            case RemovePolicy.WhenSourceAgentNull:
                if (!task.IsSourceValid)
                    RemoveTask(task);
                break;
            case RemovePolicy.WhenSourceTaskRemoved:
                if (task.SourceTaskStatus() == RunningStatus.Removed)
                    RemoveTask(task);
                break;
            case RemovePolicy.WhenSourceTaskExited:
                if (task.SourceTaskStatus() == RunningStatus.Succeed 
                || task.SourceTaskStatus() == RunningStatus.Failed)
                    RemoveTask(task);
                break;
            case RemovePolicy.WhenSourceTaskFailed:
                if (task.SourceTaskStatus() == RunningStatus.Failed)
                    RemoveTask(task);
                break;
            case RemovePolicy.WhenSourceTaskSucceed:
                if (task.SourceTaskStatus() == RunningStatus.Succeed)
                    RemoveTask(task);
                break;
        }
    }
}