using GPC.Job;

namespace GPC.Scheduler;

public interface IScheduler
{
    void AddJob(IJob job);
    void RemoveJob(IJob job);
    void Update(double delta);
    void PhysicsUpdate(double delta);
}