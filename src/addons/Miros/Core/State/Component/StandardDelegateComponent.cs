using System;

namespace Miros.Core;

public class StandardDelegateComponent: IStateComponent<JobBase>
{
    public Func<State, bool> EnterCondition { get; init; }
    public Func<State, bool> ExitCondition { get; init; }
    public Action<State> EnterFunc { get; init; }
    public Action<State> ExitFunc { get; init; }
    public Action<State> OnSucceedFunc { get; init; }
    public Action<State> OnFailedFunc { get; init; }
    public Action<State> PauseFunc { get; init; }
    public Action<State> ResumeFunc { get; init; }
    public Action<State> OnStackFunc { get; init; }
    public Action<State> OnStackOverflowFunc { get; init; }
    public Action<State> OnDurationOverFunc { get; init; }
    public Action<State> OnPeriodOverFunc { get; init; }
    public Action<State, double> UpdateFunc { get; init; }
    public Action<State, double> PhysicsUpdateFunc { get; init; }

    public void Activate(JobBase job)
    {
        RegisterEvents(job);
    }

    public void Deactivate(JobBase job)
    {
        UnregisterEvents(job);
    }


    public void RegisterEvents(JobBase job)
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

    public void UnregisterEvents(JobBase job)
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