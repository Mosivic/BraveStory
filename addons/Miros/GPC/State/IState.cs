using System;
using GPC.Scheduler;

namespace GPC.States;

public interface IState
{
    string Id { get; set; }
    string Name { get; set; }
    string Layer { get; set; }
    int Priority { get; set; }
    Type Type { get; set; }
    Status Status { get; set; }
    IScheduler Scheduler { get; set; }
}