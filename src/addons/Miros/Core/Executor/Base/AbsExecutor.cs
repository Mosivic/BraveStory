using System.Collections.Generic;

namespace Miros.Core;

public abstract class AbsExecutor<TState>
    where TState : State
{
    protected readonly Dictionary<Tag, TState> _states = []; // 所有状态(包括临时状态)
    protected readonly Dictionary<Tag, TState> _tempStates = []; // 临时状态
}
