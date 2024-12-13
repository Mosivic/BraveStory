using System.Collections.Generic;

namespace Miros.Core;

public abstract class AbsExecutor<TTask>
    where TTask : TaskBase
{
    protected readonly Dictionary<Tag, TTask> _tasks = []; // 所有任务(包括临时任务)
    protected readonly Dictionary<Tag, TTask> _tempTasks = []; // 临时任务
}
