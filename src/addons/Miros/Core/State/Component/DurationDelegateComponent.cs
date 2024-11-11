using System;

namespace Miros.Core;

public readonly struct DurationDelegateComponent : IStateComponent<NativeJob>
{
    public Action<NativeState> OnDurationOverFunc { get; init; }
    public Action<NativeState> OnPeriodOverFunc { get; init; }

    public void RegisterHandler(NativeJob job)
    {
        job.OnDurationOver += OnDurationOverFunc;
        job.OnPeriodOver += OnPeriodOverFunc;
    }

    public void UnregisterHandler(NativeJob job)
    {
        job.OnDurationOver -= OnDurationOverFunc;
        job.OnPeriodOver -= OnPeriodOverFunc;
    }
}