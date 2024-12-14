namespace Miros.Core;

public interface IExecutor
{
    void AddTask(ITask task, Context context);
    void RemoveTask(ITask task);
    ITask GetNowTask(Tag layer);
    ITask GetLastTask(Tag layer);
    ITask[] GetAllTasks();
    bool HasTaskRunning(ITask task);
    void Update(double delta);
    void PhysicsUpdate(double delta);
    void SwitchTaskByTag(Tag tag, Context context); 

}