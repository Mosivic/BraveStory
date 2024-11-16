using System;

namespace Miros.Core;

public class EffectDelegateComponent : StateComponent<EffectTask>
{
    public Action<EffectTask> OnDurationOveredFunc { get; init; }
    public Action<EffectTask> OnPeriodOveredFunc { get; init; }
    public Action<EffectTask> OnStackedFunc { get; init; }
    public Action<EffectTask> OnStackOverflowedFunc { get; init; }

    public override void Activate(EffectTask task)
    {
        RegisterEvents(task);
    }

    public override void Deactivate(EffectTask task)
    {
        UnregisterEvents(task);
    }

    public void RegisterEvents(EffectTask task)
    {
        task.OnDurationOvered += OnDurationOveredFunc;
        task.OnPeriodOvered += OnPeriodOveredFunc;
        task.OnStacked += OnStackedFunc;
        task.OnStackOverflowed += OnStackOverflowedFunc;
    }

    public void UnregisterEvents(EffectTask task)
    {
        task.OnDurationOvered -= OnDurationOveredFunc;
        task.OnPeriodOvered -= OnPeriodOveredFunc;
        task.OnStacked -= OnStackedFunc;
        task.OnStackOverflowed -= OnStackOverflowedFunc;
    }
}