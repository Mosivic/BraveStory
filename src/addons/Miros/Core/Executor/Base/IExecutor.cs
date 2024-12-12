namespace Miros.Core;

public interface IExecutor
{
    void AddTask(ITask task, Context context);
    void RemoveTask(ITask task);
    ITask GetNowTask(Tag layer);
    ITask GetLastTask(Tag layer);
    ITask[] GetAllTasks();
    double GetCurrentTaskTime(Tag layer);
    bool HasTaskRunning(ITask task);
    void Update(double delta);
    void PhysicsUpdate(double delta);
}