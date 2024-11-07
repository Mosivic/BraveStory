
namespace Miros.Core;

public interface IScheduler
{
    void AddJob(IJob job);
    void RemoveJob(IJob job);
    AbsState GetNowState(GameplayTag layer);
    AbsState GetLastState(GameplayTag layer);
    double GetCurrentStateTime(GameplayTag layer);
    bool HasJobRunning(IJob job);
    void Update(double delta);
    void PhysicsUpdate(double delta);
}