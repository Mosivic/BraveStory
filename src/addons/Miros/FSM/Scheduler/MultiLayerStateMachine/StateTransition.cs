using System.Collections.Generic;
using FSM.States;

public class StateTransition
{
    public AbsState FromState { get; }
    public AbsState ToState { get; }
    public GameplayTagQuery Conditions { get; }
    
    public StateTransition(AbsState fromState, AbsState toState,GameplayTagQuery conditions)
    {
        FromState = fromState;
        ToState = toState;
        Conditions = conditions;
    }
    
    public bool CanTransition(GameplayTagContainer ownedTags)
    {
        return Conditions?.Matches(ownedTags) ?? true;
    }
} 