using System;

namespace Miros.Core;

public readonly struct StackDelegateComponent : IStateComponent<NativeJob>
{

    public Action<NativeState> OnStackFunc { get; init; }
    public Action<NativeState> OnStackOverflowFunc { get; init; }
    public Action<NativeState> OnDurationOverFunc { get; init; }
    public Action<NativeState> OnPeriodOverFunc { get; init; }

    public void RegisterHandler(NativeJob job)
    {
        job.OnStack += OnStackFunc;
        job.OnStackOverflow += OnStackOverflowFunc;
        job.OnDurationOver += OnDurationOverFunc;
        job.OnPeriodOver += OnPeriodOverFunc;
    }

    public void UnregisterHandler(NativeJob job)
    {
        job.OnStack -= OnStackFunc;
        job.OnStackOverflow -= OnStackOverflowFunc;
        job.OnDurationOver -= OnDurationOverFunc;
        job.OnPeriodOver -= OnPeriodOverFunc;
    }
}