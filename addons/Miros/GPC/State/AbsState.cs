using System;
using System.Collections.Generic;

namespace GPC.States;

public abstract class AbsState
{
    // Core
    public string Name { get; set; }
    public Layer Layer { get; set; }
    public Type Type { get; set; }
    public int Priority { get; set; }
    public IGpcToken Source { get; set; }
    public JobRunningStatus Status { get; set; } = JobRunningStatus.NoRun;
    // Duration
    public double Duration { get; set; } = 0;
    public double DurationElapsed { get; set; } 
    // Period
    public double Period { get; set; } = 0;
    public double PeriodElapsed { get; set; }
    // Stack
    public int StackMaxCount { get; set; } = 1;
    public int StackCurrentCount { get; set; } = 1;
    public Dictionary<IGpcToken,int> StackSourceCountDict { get; set; } 
    public StateStackType StackType { get; set; } = StateStackType.None;
    // Function
    public bool UsePrepareFuncAsRunCondition { get; set; } = true;
    public Func<bool> IsPreparedFunc { get; set; }
    public Func<bool> IsSucceedFunc { get; set; }
    public Func<bool> IsFailedFunc { get; set; }
    public Action<AbsState> StartFunc { get; set; }
    public Action<AbsState> SucceedFunc { get; set; }
    public Action<AbsState> FailedFunc { get; set; }
    public Action<AbsState> PauseFunc { get; set; }
    public Action<AbsState> ResumeFunc { get; set; }
    public Action<AbsState> StackFunc { get; set; }
    public Action<AbsState> StackOverflowFunc { get; set; }
    public Action<AbsState> PeriodFunc { get; set; }
    public Action<AbsState> RunningFunc { get; set; }
    public Action<AbsState> RunningPhysicsFunc { get; set; }
}