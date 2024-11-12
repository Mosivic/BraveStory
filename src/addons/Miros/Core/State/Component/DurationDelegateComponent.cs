using System;

namespace Miros.Core;

public readonly struct EffectDelegateComponent : IStateComponent<EffectJob>
{
    public Action<EffectJob> OnDurationOveredFunc { get; init; }
    public Action<EffectJob> OnPeriodOveredFunc { get; init; }
    public Action<EffectJob> OnStackedFunc { get; init; }
    public Action<EffectJob> OnStackOverflowedFunc { get; init; }

    public void Activate(EffectJob job)
    {
        RegisterEvents(job);
    }

    public void Deactivate(EffectJob job)
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