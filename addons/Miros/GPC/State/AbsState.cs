using System;
using System.Collections.Generic;

namespace GPC.States;

public abstract class AbsState
{
    // Core
    public string Name { get; init; }
    public Layer Layer { get; init; }
    public Type Type { get; init; }
    public int Priority { get; init; }
    public IGpcToken Source { get; init; }
    public JobRunningStatus Status { get; set; } = JobRunningStatus.NoRun;
    // Duration
    public double Duration { get; init; } = 0;
    public double DurationElapsed { get; set; } 
    // Period
    public double Period { get; init; } = 0;
    public double PeriodElapsed { get; set; }
    // Stack
    public int StackMaxCount { get; init; } = 1;
    public int StackCurrentCount { get; set; } = 1;
    public Dictionary<IGpcToken,int> StackSourceCountDict { get; set; }
    public bool IsStack { get; init; } = false;
    public StateStackType StackType { get; init; } = StateStackType.Target;
    // Function
    public bool UsePrepareFuncAsRunCondition { get; init; } = true;
    public Func<bool> IsPreparedFunc { get; init; }
    public Func<bool> IsSucceedFunc { get; init; }
    public Func<bool> IsFailedFunc { get; init; }
    public Action<AbsState> StartFunc { get; init; }
    public Action<AbsState> SucceedFunc { get; init; }
    public Action<AbsState> FailedFunc { get; init; }
    public Action<AbsState> PauseFunc { get; init; }
    public Action<AbsState> ResumeFunc { get; init; }
    public Action<AbsState> StackFunc { get; init; }
    public Action<AbsState> StackOverflowFunc { get; init; }
    public Action<AbsState> StackExpirationFunc { get; init; }
    public Action<AbsState> PeriodFunc { get; init; }
    public Action<AbsState> RunningFunc { get; init; }
    public Action<AbsState> RunningPhysicsFunc { get; init; }
}