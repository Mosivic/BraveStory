using System.Collections.Generic;

namespace Miros.Core;

public enum TaskType
{
    Base,
    Effect,
    Random,
    Parallel,
    Serial
}

public class TaskProvider
{
    private static readonly Dictionary<TaskType, ITask> _taskCache = [];

    public static ITask GetTask(TaskType taskType)
    {
        if (_taskCache.TryGetValue(taskType, out var task))
            return task;

        switch (taskType)
        {
            case TaskType.Base:
                task = new TaskBase<State>();
                break;
            case TaskType.Effect:
                task = new EffectTask();
                break;
            case TaskType.Random:
                task = new RandomTask();
                break;
            case TaskType.Parallel:
                task = new ParallelTask();
                break;
            case TaskType.Serial:
                task = new SerialTask();
                break;
        }

        _taskCache[taskType] = task;
        return task;
    }


    // private static TTask GetTask<TTask>(Type taskType) where TTask : TaskBase<State>
    // {
    //     return CreateTask<TTask>(taskType);
    // }


    // private static TaskBase<State> GetTask(State state)
    // {
    //     return CreateTask(state);
    // }


    // private static TaskBase<State> CreateTask(State state)
    // {
    //     var taskType = state.TaskType;
    //     var task = (TaskBase<State>)Activator.CreateInstance(taskType, state);
    //     return task;
    // }
}