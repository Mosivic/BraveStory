namespace Miros.Core;

public interface IScheduler<TTask>
    where TTask : TaskBase
{
    void AddTask(TTask task);
    void RemoveTask(TTask task);
    TTask GetNowTask(Tag layer);
    TTask GetLastTask(Tag layer);
    TTask[] GetAllTasks();
    double GetCurrentTaskTime(Tag layer);
    bool HasTaskRunning(TTask task);
    void Update(double delta);
    void PhysicsUpdate(double delta);
}