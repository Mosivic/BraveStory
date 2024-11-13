using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;
public class StateTransitionContainer
{
    private readonly Dictionary<Tag, HashSet<StateTransition>> _transitions = [];
    private readonly HashSet<StateTransition> _anyTransitions = [];
    

    public void AddTransition(Tag fromJobSign,Tag toJobSign,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        var transition = new StateTransition(toJobSign,condition,mode);
        if(!_transitions.ContainsKey(fromJobSign))
        {
            _transitions[fromJobSign] = [];
        }
        _transitions[fromJobSign].Add(transition);
    }

    public void AddAnyTransition(Tag toJobSign,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        var transition = new StateTransition(toJobSign,condition,mode);
        _anyTransitions.Add(transition);
    }

    
    public bool CanTransitionTo(Tag fromJobSign, Tag toJobSign)
    {
        if (!_transitions.TryGetValue(fromJobSign, out var rules))
        {
            return false;
        }
        
        var rule = rules.FirstOrDefault(r => r.ToJobSign == toJobSign);
        return rule != null && rule.CanTransition();
    }
    
    // 返回满足转换条件的所有状态
    public IEnumerable<StateTransition> GetPossibleTransition(Tag fromJobSign)
    {
        if (!_transitions.TryGetValue(fromJobSign, out var rules))
        {
            return Enumerable.Empty<StateTransition>();
        }
        
        return rules.Union(_anyTransitions).Where(r => r.CanTransition());
    }

    public IEnumerable<Tag> GetAllJobSigns()
    {
        var fromTransitions = _transitions.Values
            .SelectMany(transitions => transitions.Select(t => t.ToJobSign));
        
        var anyTransitions = _anyTransitions.Select(t => t.ToJobSign);
        
        return fromTransitions.Union(anyTransitions);
    }


    // 添加链式调用方法
    public StateTransitionContainer AddTransitionGroup(Tag fromJobSign, IEnumerable<StateTransition> transitions)
    {
        foreach (var transition in transitions)
        {
            AddTransition(fromJobSign, transition.ToJobSign, transition.Condition, transition.Mode);
        }
        return this;
    }

}
