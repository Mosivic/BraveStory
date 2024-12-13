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
    }

    public virtual ITask[] GetAllTasks()
    {
        return [.. _tasks.Values];
    }

    private void RemoveTempTasks()
    {
        foreach (var task in _tempTasks.Values)
        {
            var removePolicy = task.RemovePolicy;
            switch (removePolicy)
            {
                case RemovePolicy.None:
                    break;
                case RemovePolicy.Condition:
                    if (task.CanRemove())
                        _tasks.Remove(task.Tag);
                    break;
                case RemovePolicy.WhenFailed:
                    if (task.Status == RunningStatus.Failed)
                        _tasks.Remove(task.Tag);
                    break;
                case RemovePolicy.WhenSucceed:
                    if (task.Status == RunningStatus.Succeed)
                        _tasks.Remove(task.Tag);
                    break;
                case RemovePolicy.WhenExited:
                    if (task.Status == RunningStatus.Succeed 
                    || task.Status == RunningStatus.Failed)
                        _tasks.Remove(task.Tag);
                    break;
                case RemovePolicy.WhenSourceAgentNull:
                    if (!task.IsSourceValid)
                        _tasks.Remove(task.Tag);
                    break;
                case RemovePolicy.WhenSourceTaskRemoved:
                    if (task.SourceTask == null)
                        _tasks.Remove(task.Tag);
                    break;
                case RemovePolicy.WhenSourceTaskExited:
                    if (task.SourceTask.IsExited)
                        _tasks.Remove(task.Tag);
                    break;
                case RemovePolicy.WhenSourceTaskFailed:
                    if (task.SourceTask.IsFailed)
                        _tasks.Remove(task.Tag);
                    break;
                case RemovePolicy.WhenSourceTaskSucceed:
                    if (task.SourceTask.IsSucceed)
                        _tasks.Remove(task.Tag);
                    break;
            }
            
            _tasks.Remove(task.Tag);
        }

    }
}