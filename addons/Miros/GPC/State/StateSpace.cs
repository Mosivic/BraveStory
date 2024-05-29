using System.Collections.Generic;

namespace GPC.State;

public class StateSpace
{
    public StateSpace()
    {
        States = new List<IState>();
    }

    public List<IState> States { get; set; }

    public StateSpace Add<S>(S s) where S : IState
    {
        States.Add(s);
        return this;
    }
}