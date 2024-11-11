using System;

namespace Miros.Core;

public readonly struct StandardDelegateComponent<T> : IStateComponent where T : AbsState<T>
{
    public Func<T, bool> EnterCondition { get; init; }
    public Func<T, bool> ExitCondition { get; init; }
    public Action<T> EnterFunc { get; init; }
    public Action<T> ExitFunc { get; init; }
    public Action<T> OnSucceedFunc { get; init; }
    public Action<T> OnFailedFunc { get; init; }
    public Action<T> PauseFunc { get; init; }
    public Action<T> ResumeFunc { get; init; }
    public Action<T> OnStackFunc { get; init; }
    public Action<T> OnStackOverflowFunc { get; init; }
    public Action<T> OnDurationOverFunc { get; init; }
    public Action<T> OnPeriodOverFunc { get; init; }
    public Action<T, double> UpdateFunc { get; init; }
    public Action<T, double> PhysicsUpdateFunc { get; init; }
}