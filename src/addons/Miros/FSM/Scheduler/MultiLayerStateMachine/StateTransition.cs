using System;
using System.Collections.Generic;
using FSM.States;

public class StateTransition
{
    public AbsState FromState { get; }
    public AbsState ToState { get; }
    public Func<bool> Condition { get; }
    public StateTransitionMode Mode { get; }
    public StateTransition(AbsState fromState, AbsState toState,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        FromState = fromState;
        ToState = toState;
        Condition = condition;
        Mode = mode;
    }
    
    public bool CanTransition()
    {
        if(Condition == null) return true;
        return Condition.Invoke();
    }
} 