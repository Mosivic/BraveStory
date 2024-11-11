using System;

namespace Miros.Core;

public class StandardDelegateComponent: IStateComponent<NativeJob>
{
    public Func<NativeState, bool> EnterCondition { get; init; }
    public Func<NativeState, bool> ExitCondition { get; init; }
    public Action<NativeState> EnterFunc { get; init; }
    public Action<NativeState> ExitFunc { get; init; }
    public Action<NativeState> OnSucceedFunc { get; init; }
    public Action<NativeState> OnFailedFunc { get; init; }
    public Action<NativeState> PauseFunc { get; init; }
    public Action<NativeState> ResumeFunc { get; init; }
    public Action<NativeState> OnStackFunc { get; init; }
    public Action<NativeState> OnStackOverflowFunc { get; init; }
    public Action<NativeState> OnDurationOverFunc { get; init; }
    public Action<NativeState> OnPeriodOverFunc { get; init; }
    public Action<NativeState, double> UpdateFunc { get; init; }
    public Action<NativeState, double> PhysicsUpdateFunc { get; init; }
    

    public void RegisterHandler(NativeJob job)
    {
        job.OnEntered += EnterFunc;
        job.OnExited += ExitFunc;
        job.OnSucceeded += OnSucceedFunc;
        job.OnFailed += OnFailedFunc;
        job.OnPaused += PauseFunc;
        job.OnResumed += ResumeFunc;
        job.OnUpdated += UpdateFunc;
        job.OnPhysicsUpdated += PhysicsUpdateFunc;
        job.EnterCondition += EnterCondition;
        job.ExitCondition += ExitCondition;
    }

    public void UnregisterHandler(NativeJob job)
    {
        job.OnEntered -= EnterFunc;
        job.OnExited -= ExitFunc;
        job.OnSucceeded -= OnSucceedFunc;
        job.OnFailed -= OnFailedFunc;
        job.OnPaused -= PauseFunc;
        job.OnResumed -= ResumeFunc;
        job.OnUpdated -= UpdateFunc;
        job.OnPhysicsUpdated -= PhysicsUpdateFunc;
        job.EnterCondition -= EnterCondition;
        job.ExitCondition -= ExitCondition;
    }
}