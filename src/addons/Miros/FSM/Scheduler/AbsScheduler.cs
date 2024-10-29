using System.Collections.Generic;
using IJob = FSM.Job.IJob;

namespace FSM.Scheduler;

public abstract class AbsScheduler
{
    protected readonly Dictionary<Layer, List<IJob>> RunningJobs = new();
    protected Dictionary<Layer, List<IJob>> WaitingJobs { get; set; } = new();
}