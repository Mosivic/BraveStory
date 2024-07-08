using System;
using System.Collections.Generic;
using GPC.States;

namespace GPC.Scheduler;

public abstract class AbsScheduler()
{
    public Action<AbsState> StateChanged;
    public Action<AbsState> StatePrepared;
    public Dictionary<Layer, List<AbsState>> States { get; set; } = new();
    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}