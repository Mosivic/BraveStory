using System.Collections.Generic;
using GPC.Job;
using GPC.Job.Config;

namespace GPC.AI;

public abstract class AbsScheduler<T>(List<T> states, ConditionLib<T> conditionLib) where T : IState
{
    protected ConditionLib<T> ConditionLib = conditionLib;
    protected JobWrapper<T> JobWrapper = new();
    protected List<T> States = states;


    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}