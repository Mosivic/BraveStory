using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using NUnit.Framework;

namespace Miros.Core;

// _tasks 即为运行的 EffectTask
public class EffectExecutor(Agent agent) : ExecutorBase<EffectTask>
{
    private readonly Agent _agent = agent;
    private readonly List<EffectTask> _runningTasks = [];

    public List<EffectTask> GetRunningTasks() => _runningTasks;

    public override void Update(double delta)
    {
        UpdateRunningEffects();

        foreach (var task in _runningTasks) task.Update(delta);
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
                _runningTasks.Add(task);
                _onRunningEffectTasksIsDirty?.Invoke(this, task);
            }
        }

        var couldExitTasks = _runningTasks.Where(task => task.CanExit()).ToList();
        foreach (var task in couldExitTasks)
        {
            task.Deactivate();
            task.Exit();
            _runningTasks.Remove(task);
            _onRunningEffectTasksIsDirty?.Invoke(this, task);
        }
    }

    private static bool AreTaskCouldStackByAnotherTask(EffectTask task, EffectTask otherTask)
    {
        return task.Stacking != null && otherTask.Stacking != null &&
        task.Stacking.StackingGroupTag == otherTask.Stacking.StackingGroupTag;
    }

    public override void AddTask(ITask task)
    {
        var effectTask = (EffectTask)task;

        if (!effectTask.CanEnter()) return;

        bool isAddTask = true;

        foreach (var existingTask in _runningTasks)
        {
            if (AreTaskCouldStackByAnotherTask(effectTask, existingTask)) 
            {
                // 如果Tag相同且来自同一个Agent, 则不添加, 并跳过当前循环
                if(effectTask.Tag == existingTask.Tag && _agent.AreTasksFromSameSource(effectTask, existingTask))
                {
                    existingTask.Stack(true);
                    isAddTask = false;
                    continue;
                }

                // 如果来自不同Agent, 则根据是否可以叠加来决定是否添加
                if(_agent.AreTasksFromSameSource(effectTask, existingTask))
                    existingTask.Stack(true);
                else
                    existingTask.Stack(false);
            }
        }

        if(isAddTask) base.AddTask(task);
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