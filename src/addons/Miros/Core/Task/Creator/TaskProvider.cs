using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class TaskProvider
{
    private static readonly Dictionary<Type, TaskBase<State>> _taskCache = [];

    public static TTask GetTask<TTask>(State state) where TTask : TaskBase<State>
    {
        var taskType = state.TaskType;
        if (!_taskCache.ContainsKey(taskType))
        {
            _taskCache[taskType] = CreateTask<TTask>(taskType);
        }
        return (TTask)_taskCache[taskType];
    }


    public static TTask GetTask<TTask>(Type taskType) where TTask : TaskBase<State>
    {
        return CreateTask<TTask>(taskType);
    }

    private static TTask CreateTask<TTask>(Type taskType) where TTask : TaskBase<State>
    {
        var task = (TTask)Activator.CreateInstance(taskType);
        return task;
    }

    public static TaskBase<State> GetTask(State state)
    {
        return CreateTask(state);
    }


    private static TaskBase<State> CreateTask(State state)
    {
        var taskType = state.TaskType;
        var task = (TaskBase<State>)Activator.CreateInstance(taskType, state);
        return task;
    }


}