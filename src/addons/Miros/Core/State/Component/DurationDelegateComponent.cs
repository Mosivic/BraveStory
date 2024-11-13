using System;

namespace Miros.Core;

public class EffectDelegateComponent : StateComponent<EffectJob>
{
    public Action<EffectJob> OnDurationOveredFunc { get; init; }
    public Action<EffectJob> OnPeriodOveredFunc { get; init; }
    public Action<EffectJob> OnStackedFunc { get; init; }
    public Action<EffectJob> OnStackOverflowedFunc { get; init; }

    public override void Activate(EffectJob job)
    {
        RegisterEvents(job);
    }

    public override void Deactivate(EffectJob job)
    {
        UnregisterEvents(job);
    }

    public void RegisterEvents(EffectJob job)
    {
        job.OnDurationOvered += OnDurationOveredFunc;
        job.OnPeriodOvered += OnPeriodOveredFunc;
        job.OnStacked += OnStackedFunc;
        job.OnStackOverflowed += OnStackOverflowedFunc;
    }

    public void UnregisterEvents(EffectJob job)
    {
        job.OnDurationOvered -= OnDurationOveredFunc;
        job.OnPeriodOvered -= OnPeriodOveredFunc;
        job.OnStacked -= OnStackedFunc;
        job.OnStackOverflowed -= OnStackOverflowedFunc;
    }
}