using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class StaticITaskCreator : ITaskCreator
{
    
    public TaskBase GetTask(State state)
    {
        return CreateTask(state.GetType());
    }

    public TaskBase GetTask(Type taskType)
    {
        return CreateTask(taskType);
    }

    
    private TaskBase CreateTask(Type taskType)
    {
        var task = (TaskBase)Activator.CreateInstance(taskType);
        return task;
    }
}