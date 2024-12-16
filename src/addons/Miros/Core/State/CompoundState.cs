using System;
using System.Collections.Generic;

namespace Miros.Core;

public class CompoundStateWithTaskBased : State
{
    public virtual Type[] SubTaskTypes { get; set; }
}

public class CompoundStateWithStateBased : State
{
    public virtual State[] SubStates { get; set; }
}