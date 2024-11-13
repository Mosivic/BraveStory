using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;
// _jobs 即为运行的 EffectJob
public class EffectScheduler : SchedulerBase<EffectJob>
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
    // 如果可以堆叠,检查是否存在相同StackingGroupTag的Job，如果存在，则调用其Stack方法
    // 如果当前Jobs不存在传入的Job，则将传入的Job添加到Jobs中，并调用其Activate和Enter方法
    public override void AddJob(EffectJob job)
    {
        if(!job.CanEnter()) return;

        var stackingComponent = job.GetComponent<StackingComponent>();
        var hasStackingComponent = stackingComponent != null;
        var hasSameJob = false;

        foreach(var _job in _jobs)
        {
            if(JobEqual(_job, job)) 
                hasSameJob = true;

            if(hasStackingComponent && _job.CanStack(stackingComponent.StackingGroupTag))
                _job.Stack();
        }

        if(!hasSameJob)
        {
            base.AddJob(job);
            job.Activate();
            job.Enter();
        }
    }

    public bool JobEqual(EffectJob job1, EffectJob job2)
    {
        return job1.Sign == job2.Sign;
    }
 
}
