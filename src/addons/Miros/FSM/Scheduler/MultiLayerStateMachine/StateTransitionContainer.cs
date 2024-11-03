using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using FSM.States;

public class StateTransitionContainer
{
    private readonly Dictionary<AbsState, HashSet<StateTransition>> _transitions = new();
    
    public void AddTransition(StateTransition transition)
    {
        if(!_transitions.ContainsKey(transition.FromState))
        {
            _transitions[transition.FromState] = new();
        }
        _transitions[transition.FromState].Add(transition);
    }

    
    public bool CanTransitionTo(GameplayTagContainer ownedTags,AbsState fromState, AbsState toState)
    {
        if (!_transitions.TryGetValue(fromState, out var rules))
        {
            return false;
        }
        
        var rule = rules.FirstOrDefault(r => r.ToState == toState);
        return rule != null && rule.CanTransition(ownedTags);
    }
    
    public IEnumerable<AbsState> GetPossibleState(GameplayTagContainer ownedTags,AbsState fromState)
    {
        if (!_transitions.TryGetValue(fromState, out var rules))
        {
            return Enumerable.Empty<AbsState>();
        }
        
        return rules.Where(r => r.CanTransition(ownedTags))
                    .Select(r => r.ToState);
    }
}
