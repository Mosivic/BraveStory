using System;

namespace Miros.Core;

public class StandardDelegateComponent: StateComponent<JobBase>
{
    public Func<State, bool> EnterCondition { get; set; }
    public Func<State, bool> ExitCondition { get; set; }
    public Action<State> EnterFunc { get; set; }
    public Action<State> ExitFunc { get; set; }
    public Action<State> OnSucceedFunc { get; set; }
    public Action<State> OnFailedFunc { get; set; }
    public Action<State> PauseFunc { get; set; }
    public Action<State> ResumeFunc { get; set; }
    public Action<State> OnStackFunc { get; set; }
    public Action<State> OnStackOverflowFunc { get; set; }
    public Action<State> OnDurationOverFunc { get; set; }
    public Action<State> OnPeriodOverFunc { get; set; }
    public Action<State, double> UpdateFunc { get; set; }
    public Action<State, double> PhysicsUpdateFunc { get; set; }

    public override void Activate(JobBase job)
    {
        RegisterEvents(job);
    }

    public override void Deactivate(JobBase job)
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