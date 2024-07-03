using System.Collections.Generic;

namespace GPC.States;

public class StateSet
{
    public StateSet()
    {
        States = new List<AbsState>();
    }

    public List<AbsState> States { get; set; }

    public StateSet Add<S>(S s) where S : AbsState
    {
        States.Add(s);
        return this;
    }
}