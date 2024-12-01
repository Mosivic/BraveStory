using System;
using System.Collections.Generic;

namespace Miros.Core;

// _tasks 即为运行的 EffectTask
public class EffectExecutor : ExecutorBase<EffectTask>
{
    private List<EffectTask> _runningTasks = [];

    public override void Update(double delta)
    {
        UpdateRunningEffects();

        foreach (var task in _runningTasks)
        {
            task.Update(delta);
        }   
    }

    private void UpdateRunningEffects()
    {
        foreach (var task in _tasks.Values)
        {
            if (task.CanEnter())
            {
                task.Activate();
                task.Enter();
                _runningTasks.Add(task);
            }
        }

        foreach (var task in _runningTasks)
        {
            if (task.CanExit())
            {
                task.Deactivate();
                task.Exit();
                _runningTasks.Remove(task);
                _tasks.Remove(task.Tag);
            }
        }
    }


    // 如果存在StackingComponent组件，则检查是否可以堆叠
    // 如果可以堆叠,检查是否存在相同StackingGroupTag的Task，如果存在，则调用其Stack方法
    // 如果当前Tasks不存在传入的Task，则将传入的Task添加到Tasks中，并调用其Activate和Enter方法
    public override void AddTask(EffectTask task)
    {
        if (!task.CanEnter()) return;

        var stackingComponent = task.GetComponent<StackingComponent>();
        var hasStackingComponent = stackingComponent != null;
        var hasSameTask = false;

        foreach (var _task in _tasks.Values)
        {
            if (TaskEqual(_task, task))
                hasSameTask = true;

            if (hasStackingComponent && _task.CanStack(stackingComponent.StackingGroupTag))
                _task.Stack();
        }

        if (!hasSameTask)
        {
            base.AddTask(task);
            task.Activate();
            task.Enter();
        }
    }

    public bool TaskEqual(EffectTask task1, EffectTask task2)
    {
        return task1.Tag == task2.Tag;
    }
}