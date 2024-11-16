using System;

namespace Miros.Core;

public class StandardDelegateComponent : StateComponent<TaskBase>
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

    public override void Activate(TaskBase task)
    {
        RegisterEvents(task);
    }

    public override void Deactivate(TaskBase task)
    {
        UnregisterEvents(task);
    }


    public void RegisterEvents(TaskBase task)
    {
        task.OnEntered += EnterFunc;
        task.OnExited += ExitFunc;
        task.OnSucceeded += OnSucceedFunc;
        task.OnFailed += OnFailedFunc;
        task.OnPaused += PauseFunc;
        task.OnResumed += ResumeFunc;
        task.OnUpdated += UpdateFunc;
        task.OnPhysicsUpdated += PhysicsUpdateFunc;
        task.EnterCondition += EnterCondition;
        task.ExitCondition += ExitCondition;
    }

    public void UnregisterEvents(TaskBase task)
    {
        task.OnEntered -= EnterFunc;
        task.OnExited -= ExitFunc;
        task.OnSucceeded -= OnSucceedFunc;
        task.OnFailed -= OnFailedFunc;
        task.OnPaused -= PauseFunc;
        task.OnResumed -= ResumeFunc;
        task.OnUpdated -= UpdateFunc;
        task.OnPhysicsUpdated -= PhysicsUpdateFunc;
        task.EnterCondition -= EnterCondition;
        task.ExitCondition -= ExitCondition;
    }
}