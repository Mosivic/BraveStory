using System;
using Godot;

namespace Miros.Core;

public class TaskCreator
{
    public static TTask GetTask<TTask>(State state) where TTask : TaskBase
    {
        return CreateTask<TTask>(state.GetType());
    }

    public static TTask GetTask<TTask>(Type taskType) where TTask : TaskBase
    {
        return CreateTask<TTask>(taskType);
    }

    private static TTask CreateTask<TTask>(Type taskType) where TTask : TaskBase
    {
        var task = (TTask)Activator.CreateInstance(taskType);
        return task;
    }

    public static TaskBase GetTask(State state)
    {
        return CreateTask(state);
    }

    public static Task<TState, THost, TContext, TExecuteArgs> GetTask<TState, THost, TContext, TExecuteArgs>(Type taskType)
    where TState : State, new()
    where THost : Node
    where TContext : Context
    where TExecuteArgs : ExecuteArgs
    {
        return (Task<TState, THost, TContext, TExecuteArgs>)Activator.CreateInstance(taskType);
    }

    private static TaskBase CreateTask(State state)
    {
        var type = state.Type;
        var task = (TaskBase)Activator.CreateInstance(type, state);
        return task;
    }


}