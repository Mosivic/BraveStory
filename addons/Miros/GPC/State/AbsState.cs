using System;
using System.Collections.Generic;

namespace GPC.States;

public abstract class AbsState
{
    // Core
    public required string Name { get; init; }
    public required Layer Layer { get; init; }
    public required Type Type { get; init; }
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
    public Dictionary<IGpcToken, int> StackSourceCountDict { get; set; }
    public bool IsStack { get; init; } = false;

    public StateStackType StackType { get; init; } = StateStackType.Target;

    // Function
    public bool UsePrepareFuncAsRunCondition { get; init; } = true;
    public Func<bool> IsPreparedFunc { get; init; }
    public Func<bool> IsSucceedFunc { get; init; }
    public Func<bool> IsFailedFunc { get; init; }
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
    public Action<AbsState> UpdateFunc { get; init; }
    public Action<AbsState> PhysicsUpdateFunc { get; init; }
}