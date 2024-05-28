using System;
using System.Collections.Generic;
using GPC;
using GPC.Job.Config;

class StateProxy<T> where T : IState
{
    public Dictionary<Evaluator<T>, bool> PreConditions { get; set; }
    public Dictionary<Evaluator<T>, bool> SuccessedConditions { get; set; }
    public Dictionary<Evaluator<T>, bool> FailedConditions { get; set; }
    public Action<T> EnterFunc { get; set; }
    public Action<T> ExitFunc { get; set; }
    public Action<T> PauseFunc { get; set; }
    public Action<T> ResumeFunc { get; set; }
    public Action<T> RunningFunc { get; set; }
    public Action<T> RunningPhysicsFunc { get; set; }
    public Predicate<T> IsPreparedFunc { get; set; }
    public Predicate<T> IsSucceedFunc { get; set; }
    public Predicate<T> IsFailedFunc { get; set; }
    public Action<T> EnterAttachFunc { get; set; }
    public Action<T> ExitAttachFunc { get; set; }
    public Action<T> PauseAttachFunc { get; set; }
    public Action<T> ResumeAttachFunc { get; set; }
    public Action<T> RunningAttachFunc { get; set; }
    public Action<T> RunningPhysicsAttachFunc { get; set; }
    public Action<T> RunningIntervalFunc { get; set; }
}