
namespace Miros.Core;

public interface IScheduler<TJob>
    where TJob : JobBase
{
    void AddJob(TJob job);
    void RemoveJob(TJob job);
    TJob GetNowJob(Tag layer);
    TJob GetLastJob(Tag layer);
    double GetCurrentJobTime(Tag layer);
    bool HasJobRunning(TJob job);
    void Update(double delta);
    void PhysicsUpdate(double delta);
}