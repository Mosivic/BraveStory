
namespace Miros.Core;

public interface IScheduler
{
    void AddJob(IJob job);
    void RemoveJob(IJob job);
    AbsState GetNowState(Tag layer);
    AbsState GetLastState(Tag layer);
    double GetCurrentStateTime(Tag layer);
    bool HasJobRunning(IJob job);
    void Update(double delta);
    void PhysicsUpdate(double delta);
}