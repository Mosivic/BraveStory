using System;

namespace Miros.Core;

public class StandardDelegateComponent: IStateComponent<NativeJob>
{
    public Func<AbsState, bool> EnterCondition { get; init; }
    public Func<AbsState, bool> ExitCondition { get; init; }
    public Action<AbsState> EnterFunc { get; init; }
    public Action<AbsState> ExitFunc { get; init; }
    public Action<AbsState> OnSucceedFunc { get; init; }
    public Action<AbsState> OnFailedFunc { get; init; }
    public Action<AbsState> PauseFunc { get; init; }
    public Action<AbsState> ResumeFunc { get; init; }
    public Action<AbsState> OnStackFunc { get; init; }
    public Action<AbsState> OnStackOverflowFunc { get; init; }
    public Action<AbsState> OnDurationOverFunc { get; init; }
    public Action<AbsState> OnPeriodOverFunc { get; init; }
    public Action<AbsState, double> UpdateFunc { get; init; }
    public Action<AbsState, double> PhysicsUpdateFunc { get; init; }

    public void Activate(NativeJob job)
    {
        RegisterEvents(job);
    }

    public void Deactivate(NativeJob job)
    {
        UnregisterEvents(job);
    }


    public void RegisterEvents(NativeJob job)
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

    public void UnregisterEvents(NativeJob job)
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