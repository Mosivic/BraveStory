using System;

namespace Miros.Core;

public readonly struct EffectDelegateComponent : IStateComponent<EffectJob>
{
    public Action<EffectJob> OnDurationOveredFunc { get; init; }
    public Action<EffectJob> OnPeriodOveredFunc { get; init; }
    public Action<EffectJob> OnStackedFunc { get; init; }
    public Action<EffectJob> OnStackOverflowedFunc { get; init; }


    public void RegisterHandler(EffectJob job)
    {
        job.OnDurationOvered += OnDurationOveredFunc;
        job.OnPeriodOvered += OnPeriodOveredFunc;
        job.OnStacked += OnStackedFunc;
        job.OnStackOverflowed += OnStackOverflowedFunc;
    }

    public void UnregisterHandler(EffectJob job)
    {
        job.OnDurationOvered -= OnDurationOveredFunc;
        job.OnPeriodOvered -= OnPeriodOveredFunc;
        job.OnStacked -= OnStackedFunc;
        job.OnStackOverflowed -= OnStackOverflowedFunc;
    }
}