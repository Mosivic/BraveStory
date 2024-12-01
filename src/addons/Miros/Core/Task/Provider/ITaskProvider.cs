namespace Miros.Core;

public interface ITaskProvider
{
    TaskBase GetTask(StateBase state);
    TaskBase[] GetAllTasks();
}