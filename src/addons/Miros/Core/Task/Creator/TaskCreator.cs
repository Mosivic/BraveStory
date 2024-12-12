﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class TaskCreator
{
    public static  TTask GetTask<TTask>(State state) where TTask : TaskBase
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
}