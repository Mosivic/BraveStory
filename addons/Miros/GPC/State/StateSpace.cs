
using System.Collections.Generic;
using GPC.Job.Config;

class StateSpace
{
    public List<IState> States{get;set;}

    public StateSpace()
    {
        States = new List<IState>();
    }

    public StateSpace Add<S>(S s) where S : IState
    {
        States.Add(s);
        return this;
    }


     
}