using System.Collections.Generic;

namespace Miros.Core;

public class CompoundState(Tag sign) : State(sign)
{
    public int ChildIndex { get; set; } = -1;
    public int Cost { get; set; }

    public Dictionary<object, object> Desired { get; set; }
    public List<State> SubTasks { get; set; }
}