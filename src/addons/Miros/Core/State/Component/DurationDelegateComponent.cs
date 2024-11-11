using System;

namespace Miros.Core;

public readonly struct DurationDelegateComponent<T> : IStateComponent<T> where T : AbsState<T>
{
    public Action<T> OnDurationOverFunc { get; init; }
    public Action<T> OnPeriodOverFunc { get; init; }
}