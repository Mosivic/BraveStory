using GPC.Scheduler;
using GPC.States;

namespace GPC.Job.Executor;

public interface IConnect<TJobProvider,TScheduler>  
where TJobProvider:IJobProvider
where TScheduler : IScheduler
{
    void AddState(AbsState state);
    void RemoveState(AbsState state);
    void Update(double delta);
    void PhysicsUpdate(double delta);
}



