using System.Collections.Generic;
using System.Linq;
using FSM.States;

public class StateTransitionContainer
{
    private readonly Dictionary<AbsState, HashSet<AbsState>> _transitions = new();
    
    public void AddTransition(AbsState fromState,AbsState toState)
    {
        var transition = new StateTransition(fromState,toState);
        if (!_transitions.ContainsKey(transition.FromState))
        {
            _transitions[transition.FromState] = new HashSet<AbsState>();
        }
        _transitions[transition.FromState].Add(transition.ToState);
    }

    public void AddTransitions(AbsState fromState,HashSet<AbsState> toStates){
        foreach(var state in toStates){
            AddTransition(fromState,state);
        }
    }
    
    public HashSet<AbsState> GetToStates(AbsState fromState){
        if(_transitions.ContainsKey(fromState)){
            return _transitions[fromState];
        }
        else{
            return new();
        }
    }
}