using System;
using GPC.States;

namespace GPC.Scheduler;

public abstract class AbsScheduler(StateSet stateSet)
{
    public Action<AbsState> StateChanged;
    public Action<AbsState> StatePrepared;

    public StateSet StateSet = stateSet;
    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}