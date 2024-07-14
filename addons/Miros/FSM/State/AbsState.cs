using System;
using System.Collections.Generic;

namespace FSM.States;

public abstract class AbsState
{
    // Core
    public required string Name { get; init; }
    public required Layer Layer { get; init; }
    public required Type JobType { get; init; }
    public required int Priority { get; init; }
    public object Source { get; init; }

    public RunningStatus Status { get; set; } = RunningStatus.NoRun;

    // Duration
    public double Duration { get; init; } = 0;

    public double DurationElapsed { get; set; }

    // Period
    public double Period { get; init; } = 0;
    public double PeriodElapsed { get; set; }
    
    // Transition
    public List<AbsState> FromState { get; } = [];
    
    // Immunity
    public Tag[] TagsInclusion { get; init; }
    public Tag[] TagsExclusion { get; init; }

    // Stack
    public int StackMaxCount { get; init; } = 1;
    public int StackCurrentCount { get; set; } = 1;
    public Dictionary<object, int> StackSourceCountDict { get; set; }
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
    public Action<AbsState, double> UpdateFunc { get; init; }
    public Action<AbsState, double> PhysicsUpdateFunc { get; init; }
    
    
    public void From(AbsState other)
    {
        if(!FromState.Contains(other))
            FromState.Add(other);
    }
    
}