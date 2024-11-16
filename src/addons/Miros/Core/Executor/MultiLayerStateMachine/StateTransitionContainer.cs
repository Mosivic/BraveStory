using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;
public class StateTransitionContainer
{
    private readonly Dictionary<Tag, HashSet<StateTransition>> _transitions = [];
    private readonly HashSet<StateTransition> _anyTransitions = [];
    
    private Action<State,TaskBase> _stateToTaskFunc;

    public void SetStateToTaskFunc(Action<State,TaskBase> func){
        _stateToTaskFunc = func;
    }
    

    public void AddTransition(State fromTask,State toTask,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        var transition = new StateTransition(toTask,condition,mode);
        if(!_transitions.ContainsKey(fromTaskSign))
        {
            _transitions[fromTaskSign] = [];
        }
        _transitions[fromTaskSign].Add(transition);
    }

    public void AddAnyTransition(Tag toTaskSign,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        var transition = new StateTransition(toTaskSign,condition,mode);
        _anyTransitions.Add(transition);
    }

    
    public bool CanTransitionTo(Tag fromTaskSign, Tag toTaskSign)
    {
        if (!_transitions.TryGetValue(fromTaskSign, out var rules))
        {
            return false;
        }
        
        var rule = rules.FirstOrDefault(r => r.ToTaskSign == toTaskSign);
        return rule != null && rule.CanTransition();
    }
    
    // 返回满足转换条件的所有状态
    public IEnumerable<StateTransition> GetPossibleTransition(Tag fromTaskSign)
    {
        if (!_transitions.TryGetValue(fromTaskSign, out var rules))
        {
            return Enumerable.Empty<StateTransition>();
        }
        
        return rules.Union(_anyTransitions).Where(r => r.CanTransition());
    }

    public IEnumerable<Tag> GetAllTaskSigns()
    {
        var fromTransitions = _transitions.Values
            .SelectMany(transitions => transitions.Select(t => t.ToTaskSign));
        
        var anyTransitions = _anyTransitions.Select(t => t.ToTaskSign);
        
        return fromTransitions.Union(anyTransitions);
    }


    // 添加链式调用方法
    public StateTransitionContainer AddTransitionGroup(Tag fromTaskSign, IEnumerable<StateTransition> transitions)
    {
        foreach (var transition in transitions)
        {
            AddTransition(fromTaskSign, transition.ToTaskSign, transition.Condition, transition.Mode);
        }
        return this;
    }

}