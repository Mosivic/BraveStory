using System.Collections.Generic;
using FSM.States;

public class StateTransition
{
    public AbsState FromState { get; }
    public AbsState ToState { get; }
    
    public StateTransition(AbsState fromState, AbsState toState)
    {
        FromState = fromState;
        ToState = toState;
    }
    
} 