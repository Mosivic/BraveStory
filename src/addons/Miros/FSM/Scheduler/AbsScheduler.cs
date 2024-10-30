using System.Collections.Generic;
using IJob = FSM.Job.IJob;

namespace FSM.Scheduler;

public abstract class AbsScheduler
{
    protected readonly Dictionary<GameplayTag, List<IJob>> RunningJobs = new();
    protected Dictionary<GameplayTag, List<IJob>> WaitingJobs { get; set; } = new();
}