using System.Collections.Generic;

namespace FSM.States;

public class CompoundState : AbsState
{
    public int SubIndex { get; set; } = -1;
    public required AbsState[] SubStates { get; init; }
}