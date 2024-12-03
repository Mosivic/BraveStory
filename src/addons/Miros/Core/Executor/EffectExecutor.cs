using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Miros.Core;

// _tasks 即为运行的 EffectTask
public class EffectExecutor : ExecutorBase<EffectTask>
{

    private readonly List<EffectTask> _runningPermanentTasks = [];

    public List<EffectTask> GetRunningTasks() => _runningPermanentTasks;

    public override void Update(double delta)
    {
        UpdateRunningEffects();

        foreach (var task in _runningPermanentTasks) task.Update(delta);
    }

    private void UpdateRunningEffects()
    {
        var couldEnterTasks = _tasks.Where(task => task.CanEnter()).ToList();
        foreach (var task in couldEnterTasks)
        {
            if (task.IsInstant)
            {
                task.Activate();
                task.Enter();
                task.Exit();
                task.Deactivate();
                _tasks.Remove(task);
            }
            else
            {
                task.Activate();
                task.Enter();
                _tasks.Remove(task);
                _runningPermanentTasks.Add(task);
                _onRunningEffectTasksIsDirty?.Invoke(this, task);
            }
        }

        var couldExitTasks = _runningPermanentTasks.Where(task => task.CanExit()).ToList();
        foreach (var task in couldExitTasks)
        {
            task.Deactivate();
            task.Exit();
            _runningPermanentTasks.Remove(task);
            _onRunningEffectTasksIsDirty?.Invoke(this, task);
        }
    }

    private static bool AreTaskCouldStackByAnotherTask(EffectTask task, EffectTask otherTask)
    {
        return task.GetComponent<StackingComponent>() != null && otherTask.GetComponent<StackingComponent>() != null &&
        task.GetComponent<StackingComponent>().StackingGroupTag == otherTask.GetComponent<StackingComponent>().StackingGroupTag;
    }

    public override void AddTask(ITask task)
    {
        var effectTask = (EffectTask)task;

        if (!effectTask.CanEnter()) return;

        foreach (var existingTask in _tasks)
        {

            if (_agent.AreTasksFromSameSource(effectTask, existingTask)) // 如果传入的Task与已存在Task来自同一个Agent,
            {
                if (AreTaskCouldStackByAnotherTask(effectTask, existingTask)) existingTask.Stack(true);
                if (existingTask.Tag != effectTask.Tag) base.AddTask(task);
            }
            else // 如果传入的Task与已存在Task来自不同Agent
            {
                if (AreTaskCouldStackByAnotherTask(effectTask, existingTask)) existingTask.Stack(false);
                base.AddTask(task);
            }
        }


        base.AddTask(task);
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