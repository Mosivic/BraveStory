using System;
using System.Collections.Generic;

namespace Miros.Core;

public abstract class AbsState
{
    // Core
    public required string Name { get; init; }
    public required Tag Tag {get;init;}
    public Type JobType { get; init; } = typeof(NativeJob);
    public int Priority { get; init; } = 0;

    public Persona Owner { get; protected set; }
    public Persona Source { get; protected set; }
    
    public string Description { get; init; }
    
    public RunningStatus Status { get; set; } = RunningStatus.NoRun;

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