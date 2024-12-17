using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public enum TaskType
{
    Base,
    Effect,
    Random,
    Parallel,
    Serial,
}

public class TaskProvider
{
    private static readonly Dictionary<TaskType, TaskBase<State>> _taskCache = [];

    public static TTask GetTask<TTask>(TaskType taskType) where TTask : TaskBase<State>
    {
        if(_taskCache.TryGetValue(taskType, out var task))
            return (TTask)task;

        switch(taskType)
        {
            case TaskType.Base:
                task = CreateTask<TTask>(typeof(TaskBase<State>));
                break;
            case TaskType.Effect:
                task = CreateTask<TTask>(typeof(EffectTask));
                break;
            case TaskType.Random:
                task = CreateTask<TTask>(typeof(RandomTask));
                break;
            case TaskType.Parallel:
                task = CreateTask<TTask>(typeof(ParallelTask));
                break;
            case TaskType.Serial:
                task = CreateTask<TTask>(typeof(SerialTask));
                break;
        }
        _taskCache[taskType] = task;
        return (TTask)task;
    }


    // private static TTask GetTask<TTask>(Type taskType) where TTask : TaskBase<State>
    // {
    //     return CreateTask<TTask>(taskType);
    // }

    private static TTask CreateTask<TTask>(Type taskType) where TTask : TaskBase<State>
    {
        var task = (TTask)Activator.CreateInstance(taskType);
        return task;
    }

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