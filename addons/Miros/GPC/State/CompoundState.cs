using System;
using System.Collections.Generic;

namespace GPC.States;

public class CompoundState : AbsState
{
    public int ChildIndex { get; set; } = -1;
    public int Cost { get; set; }

    public Dictionary<object, object> Desired { get; set; }
    public List<AbsState> SubJobs { get; set; }


}