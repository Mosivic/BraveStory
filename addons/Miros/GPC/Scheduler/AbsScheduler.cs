using System.Collections.Generic;
using GPC.Job;
using GPC.Job.Config;

namespace GPC.AI;

public abstract class AbsScheduler(List<State> states)
{
    protected JobWrapper<IState> JobWrapper = new();
    protected List<State> States = states;


    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}