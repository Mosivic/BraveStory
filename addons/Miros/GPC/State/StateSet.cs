using System.Collections.Generic;

namespace GPC.States;

public class StateSet
{
    public List<AbsState> States { get; set; } = new();

    public StateSet Add<TState>(TState state) where TState : AbsState
    {
        States.Add(state);
        return this;
    }
}