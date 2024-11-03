using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using FSM.States;

public class StateTransitionContainer
{
    private readonly Dictionary<AbsState, HashSet<StateTransition>> _transitions = new();
    
    public void AddTransition(AbsState fromState,AbsState toState,Func<bool> condition = null)
    {
        var transition = new StateTransition(fromState,toState,condition);
        if(!_transitions.ContainsKey(transition.FromState))
        {
            _transitions[transition.FromState] = new();
        }
        _transitions[transition.FromState].Add(transition);
    }

    
    public bool CanTransitionTo(AbsState fromState, AbsState toState)
    {
        if (!_transitions.TryGetValue(fromState, out var rules))
        {
            return false;
        }
        
        var rule = rules.FirstOrDefault(r => r.ToState == toState);
        return rule != null && rule.CanTransition();
    }
    
    public IEnumerable<AbsState> GetPossibleState(AbsState fromState)
    {
        if (!_transitions.TryGetValue(fromState, out var rules))
        {
            return Enumerable.Empty<AbsState>();
        }
        
        return rules.Where(r => r.CanTransition())
                    .Select(r => r.ToState);
    }
}
