using System;
using System.Collections.Generic;
using System.Linq;
using FSM.States;

public class StateTransitionContainer
{
    private readonly Dictionary<AbsState, HashSet<StateTransition>> _transitions = new();
    private readonly HashSet<StateTransition> _anyTransitions = new();
    
    public void AddTransition(AbsState fromState,AbsState toState,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        var transition = new StateTransition(toState,condition,mode);
        if(!_transitions.ContainsKey(fromState))
        {
            _transitions[fromState] = new();
        }
        _transitions[fromState].Add(transition);
    }

    public void AddAnyTransition(AbsState toState,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        var transition = new StateTransition(toState,condition,mode);
        _anyTransitions.Add(transition);
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
    
    // 返回满足转换条件的所有状态
    public IEnumerable<StateTransition> GetPossibleTransition(AbsState fromState)
    {
        if (!_transitions.TryGetValue(fromState, out var rules))
        {
            return Enumerable.Empty<StateTransition>();
        }
        
        return rules.Union(_anyTransitions).Where(r => r.CanTransition());
    }

    public IEnumerable<AbsState> GetAllStates()
    {
        var fromTransitions = _transitions.Values
            .SelectMany(transitions => transitions.Select(t => t.ToState));
        
        var anyTransitions = _anyTransitions.Select(t => t.ToState);
        
        return fromTransitions.Union(anyTransitions);
    }
}
