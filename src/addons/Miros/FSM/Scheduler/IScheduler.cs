using FSM.Job;

namespace FSM.Scheduler;

public interface IScheduler
{
    void AddJob(IJob job);
    void RemoveJob(IJob job);
    bool HasJobRunning(IJob job);
    void Update(double delta);
    void PhysicsUpdate(double delta);
}