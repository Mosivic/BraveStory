using System;

namespace Miros.Core;

// _tasks 即为运行的 EffectTask
public class EffectScheduler : SchedulerBase<EffectTask>
{
    private event Action OnEffectsIsDirty;

    public override void Update(double delta)
    {
    }

    public void RegisterOnEffectsIsDirty(Action action)
    {
        OnEffectsIsDirty += action;
    }

    public void UnregisterOnEffectsIsDirty(Action action)
    {
        OnEffectsIsDirty -= action;
    }


    private void UpdateEffect()
    {
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
        return task1.Sign == task2.Sign;
    }
}