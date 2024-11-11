using System;
using System.Collections.Generic;

namespace Miros.Core;

public abstract class AbsState<T> where T : AbsState<T>
{
    // Core
    public required string Name { get; init; }
    public string Description { get; init; }

    public Type JobType { get; init; } = typeof(NativeJob);
    public int Priority { get; init; } = 0;

    public Persona Owner { get; protected set; }
    public Persona Source { get; protected set; }
    public RunningStatus Status { get; set; } = RunningStatus.NoRun;

    public Dictionary<Type, IStateComponent> Components { get; set; } = new();

}