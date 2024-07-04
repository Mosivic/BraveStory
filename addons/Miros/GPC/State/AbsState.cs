using System;
using GPC.Scheduler;

namespace GPC.States;

public abstract class AbsState
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Layer { get; set; } = "Default";
    public int Priority { get; set; }
    public Type Type { get; set; }
    public Status Status { get; set; }
    public IScheduler Scheduler { get; set; }
}