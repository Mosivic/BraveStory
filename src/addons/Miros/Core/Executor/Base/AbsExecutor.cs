using System.Collections.Generic;

namespace Miros.Core;

public abstract class AbsExecutor<TTask>
    where TTask : TaskBase
{
    protected readonly Dictionary<Tag, TTask> _tasks = [];
}