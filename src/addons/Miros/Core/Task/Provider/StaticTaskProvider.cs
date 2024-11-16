using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class StaticTaskProvider : ITaskProvider
{
    private readonly Dictionary<State, TaskBase> _statesTask = [];

    public TaskBase GetTask(State state)
    {
        if (_statesTask.TryGetValue(state, out var task))
            return task;
        return CreateTask(state);
    }

    public TaskBase[] GetAllTasks()
    {
        return _statesTask.Values.ToArray();
    }

    private TaskBase CreateTask(State state)
    {
        var type = state.TaskType;
        var task = (TaskBase)Activator.CreateInstance(type, state);
        _statesTask[state] = task;
        return task;
    }
}