using System.Collections.Generic;

namespace GPC.States;

public class StateSpace
{
    public StateSpace()
    {
        States = new List<AbsState>();
    }

    public List<AbsState> States { get; set; }

    public StateSpace Add<S>(S s) where S : AbsState
    {
        States.Add(s);
        return this;
    }
}