using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

// _tasks 即为运行的 EffectTask
public class EffectExecutor : ExecutorBase<EffectTask>
{

    private readonly List<EffectTask> _runningPermanentTasks = [];

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
            if(task.IsInstant)
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


    // 如果存在StackingComponent组件，则检查是否可以堆叠
    // 如果可以堆叠,检查是否存在相同StackingGroupTag的Task，如果存在，则调用其Stack方法
    // 如果当前Tasks不存在传入的Task，则将传入的Task添加到Tasks中，并调用其Activate和Enter方法
    public override void AddTask(ITask task)
    {
        var effectTask = (EffectTask)task;
        if (!effectTask.CanEnter()) return;

        var stackingComponent = effectTask.GetComponent<StackingComponent>();
        var hasStackingComponent = stackingComponent != null;
        var hasSameTask = false;

        foreach (var _task in _tasks)
        {
            if (TaskEqual(_task, effectTask))
                hasSameTask = true;

            if (hasStackingComponent && _task.CanStack(stackingComponent.StackingGroupTag))
                _task.Stack();
        }

        if (hasSameTask) return;
        base.AddTask(task);
    }

    private bool TaskEqual(EffectTask task1, EffectTask task2)
    {
        return task1.Tag == task2.Tag;
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