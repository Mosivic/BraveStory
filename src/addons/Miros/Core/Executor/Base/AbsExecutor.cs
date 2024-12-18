using System.Collections.Generic;

namespace Miros.Core;

public abstract class AbsExecutor
{
    protected readonly Dictionary<Tag, State> _states = []; // 所有状态(包括临时状态)
    protected readonly Dictionary<Tag, State> _tempStates = []; // 临时状态
}