using System;

namespace Miros.Core;

public interface ITaskProvider
{
    TaskBase GetTask(State state);
    TaskBase[] GetAllTasks();
}