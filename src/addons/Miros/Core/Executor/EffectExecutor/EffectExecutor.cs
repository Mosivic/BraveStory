using System;
using System.Collections.Generic;

namespace Miros.Core;

// _tasks 即为运行的 EffectTask
public class EffectExecutor(Agent agent) : ExecutorBase<EffectTask>
{
    private readonly Agent _agent = agent;
    private readonly List<EffectTask> _runningTasks = [];

    private readonly List<EffectTask> _tasksRemoveCache = [];

    public List<EffectTask> GetRunningTasks()
    {
        return _runningTasks;
    }

    public override void Update(double delta)
    {
        UpdateTasks();

        foreach (var task in _runningTasks) task.Update(delta);
    }

    private void UpdateTasks()
    {
        // Enter
        foreach (var task in _tasks.Values)
        {
            if (!task.CanEnter())
            {
                if (task.RemoveSelfOnEnterFailed)
                    _tasksRemoveCache.Add(task);

                continue;
            }

            if (task.IsInstant)
            {
                task.Activate();
                task.Enter();
                task.Exit();
                task.Deactivate();

                if (!task.KeepSelfOnExitSucceeded)
                    _tasksRemoveCache.Add(task);
            }
            else
            {
                task.Activate();
                task.Enter();
                _runningTasks.Add(task);
                _tasksRemoveCache.Add(task);
                _onRunningEffectTasksIsDirty?.Invoke(this, task);
            }
        }

        // Exit
        foreach (var task in _runningTasks)
        {
            if (!task.CanExit())
                continue;

            task.Deactivate();
            task.Exit();
            _runningTasks.Remove(task);
            _onRunningEffectTasksIsDirty?.Invoke(this, task);

            if (task.KeepSelfOnExitSucceeded)
                _tasks.Add(task.Tag, task);
        }

        // Clear
        foreach (var task in _tasksRemoveCache)
            _tasks.Remove(task.Tag);
        _tasksRemoveCache.Clear();
    }

    private static bool AreTaskCouldStackByAnotherTask(EffectTask task, EffectTask otherTask)
    {
        return task.Stacking != null && otherTask.Stacking != null &&
               task.Stacking?.GroupTag == otherTask.Stacking?.GroupTag;
    }

    public override void AddTask(ITask task, Context context)
    {
        var effectTask = (EffectTask)task;
        var isAddTask = true;

        foreach (var existingTask in _runningTasks)
            if (AreTaskCouldStackByAnotherTask(effectTask, existingTask))
            {
                // 如果Tag相同且来自同一个Agent, 则不添加, 并跳过当前循环
                if (effectTask.Tag == existingTask.Tag && AreFromSameSourceAgent(effectTask, existingTask))
                {
                    existingTask.Stack(true);
                    isAddTask = false;
                    continue;
                }

                // 如果来自不同Agent, 则根据是否可以叠加来决定是否添加
                if (AreFromSameSourceAgent(effectTask, existingTask))
                    existingTask.Stack(true);
                else
                    existingTask.Stack();
            }

        if (isAddTask) base.AddTask(task, context);
    }

    private bool AreFromSameSourceAgent(EffectTask task1, EffectTask task2)
    {
        var state1 = task1.State;
        var state2 = task2.State;

        if (state1 == null || state2 == null)
            return false;
        return state1.SourceAgent == state2.SourceAgent;
    }


    #region Event

    private EventHandler<EffectTask> _onRunningEffectTasksIsDirty;

    public void RegisterOnRunningEffectTasksIsDirty(EventHandler<EffectTask> handler)
    {
        _onRunningEffectTasksIsDirty += handler;
    }

    public void UnregisterOnRunningEffectTasksIsDirty(EventHandler<EffectTask> handler)
    {
        _onRunningEffectTasksIsDirty -= handler;
    }


    #endregion
}