using System;
using System.Collections.Generic;
using FSM.Job;

namespace FSM.States;

public abstract class AbsState
{
    // Core
    public required string Name { get; init; }
    public required GameplayTag Tag {get;init;}
    public Type JobType { get; init; } = typeof(JobSimple);
    public required int Priority { get; init; }
    public object Source { get; init; }

    public RunningStatus Status { get; set; } = RunningStatus.NoRun;

    // Duration
    public double Duration { get; init; } = 0;
    public double DurationElapsed { get; set; }
    // Period
    public double Period { get; init; } = 0;
    public double PeriodElapsed { get; set; }

    // Stack
    public int StackMaxCount { get; init; } = 1;
    public int StackCurrentCount { get; set; } = 1;
    public Dictionary<object, int> StackSourceCountDict { get; set; }
    public bool IsStack { get; init; } = false;
    public StateStackType StackType { get; init; } = StateStackType.Target;

    // Function
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



}