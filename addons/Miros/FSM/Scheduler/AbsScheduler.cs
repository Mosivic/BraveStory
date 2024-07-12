using System;
using System.Collections.Generic;
using FSM.Job;
using FSM.States;

namespace FSM.Scheduler;

public abstract class AbsScheduler
{
    protected readonly Dictionary<Layer, List<IJob>> RunningJobs = new();
    public Action<AbsState> StateChanged;
    public Action<AbsState> StatePrepared;
    protected Dictionary<Layer, List<IJob>> WaitingJobs { get; set; } = new();
}