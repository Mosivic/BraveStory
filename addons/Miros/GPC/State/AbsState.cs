using System;
using GPC.Scheduler;


namespace GPC.States;

public abstract class AbsState
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Layer Layer { get; set; } = null;
    public int Priority { get; set; } = 0;
    public Type Type { get; set; }
    public Status Status { get; set; }
    public IScheduler Scheduler { get; set; }
}