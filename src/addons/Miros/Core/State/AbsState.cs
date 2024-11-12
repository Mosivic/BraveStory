using System;
using System.Collections.Generic;

namespace Miros.Core;

public class AbsState
{
    // Core
    public required string Name { get; init; }
    public string Description { get; init; }
    
    public Type JobType { get; init; } = typeof(JobBase);
    public int Priority { get; init; } = 0;
    public Tag Layer { get; init; }

    public Persona Owner { get; protected set; }
    public Persona Source { get; protected set; }
    public RunningStatus Status { get; set; } = RunningStatus.NoRun;

    public Dictionary<Type, IStateComponent<JobBase>> Components { get; set; } = new();

}