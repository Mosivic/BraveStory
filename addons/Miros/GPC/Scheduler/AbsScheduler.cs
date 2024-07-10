using System;
using System.Collections.Generic;
using GPC.Job;
using GPC.States;

namespace GPC.Scheduler;

public abstract class AbsScheduler
{
    protected readonly Dictionary<Layer, List<IJob>> RunningJobs = new();
    public Action<AbsState> StateChanged;
    public Action<AbsState> StatePrepared;
    protected Dictionary<Layer, List<IJob>> WaitingJobs { get; set; } = new();
}