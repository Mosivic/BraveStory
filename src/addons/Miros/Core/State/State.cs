using System;
using System.Collections.Generic;

namespace Miros.Core;

public class State
{
    public Tag Tag { get; init; }
    public virtual Type Type => typeof(TaskBase);
    public int Priority { get; set; } = 0;
    public Agent Owner { get; set; }
    public Agent Source { get; init; }

    public RunningStatus Status { get; set; } = RunningStatus.NoRun;
    public bool IsActive => Status == RunningStatus.Running;
    public double RunningTime { get; set; } = 0;
    
    public Func<State, bool> EnterCondition { get; set; }
    public Func<State, bool> ExitCondition { get; set; }
    public Action<State> EnterFunc { get; set; }
    public Action<State> ExitFunc { get; set; }
    public Action<State> SucceedFunc { get; set; }
    public Action<State> FailedFunc { get; set; }
    public Action<State> PauseFunc { get; set; }
    public Action<State> ResumeFunc { get; set; }
    public Action<State> StackFunc { get; set; }
    public Action<State> StackOverflowFunc { get; set; }
    public Action<State> DurationOverFunc { get; set; }
    public Action<State> PeriodOverFunc { get; set; }
    public Action<State, double> UpdateFunc { get; set; }
    public Action<State, double> PhysicsUpdateFunc { get; set; }
    
}