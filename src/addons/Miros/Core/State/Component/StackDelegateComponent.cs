using System;

namespace Miros.Core;

public readonly struct StackDelegateComponent<T> : IStateComponent where T : AbsState<T>
{

    public Action<T> OnStackFunc { get; init; }
    public Action<T> OnStackOverflowFunc { get; init; }
    public Action<T> OnDurationOverFunc { get; init; }
    public Action<T> OnPeriodOverFunc { get; init; }
}