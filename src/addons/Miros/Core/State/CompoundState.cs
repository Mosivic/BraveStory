using System.Collections.Generic;

namespace Miros.Core;

public class CompoundState : State
{
    public List<State> SubStates { get; set; }
}
