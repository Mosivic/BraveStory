using System;
using System.Collections.Generic;
using GPC.Job;
using GPC.States;

namespace GPC.Scheduler;

public abstract class AbsScheduler
{
    public Action<AbsState> StateChanged;
    public Action<AbsState> StatePrepared;
    protected Dictionary<Layer, List<IJob>> Jobs { get; set; } = new();
    protected readonly Dictionary<Layer, List<IJob>> RunningJobs = new();
    
}