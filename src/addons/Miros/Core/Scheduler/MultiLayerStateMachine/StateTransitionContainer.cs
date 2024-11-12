using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;
public class StateTransitionContainer
{
    private readonly Dictionary<JobBase, HashSet<StateTransition>> _transitions = [];
    private readonly HashSet<StateTransition> _anyTransitions = [];
    

    public void AddTransition(JobBase fromJob,JobBase toJob,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        var transition = new StateTransition(toJob,condition,mode);
        if(!_transitions.ContainsKey(fromJob))
        {
            _transitions[fromJob] = [];
        }
        _transitions[fromJob].Add(transition);
    }

    public void AddAnyTransition(JobBase toJob,Func<bool> condition = null,StateTransitionMode mode = StateTransitionMode.Normal)
    {
        var transition = new StateTransition(toJob,condition,mode);
        _anyTransitions.Add(transition);
    }

    
    public bool CanTransitionTo(JobBase fromJob, JobBase toJob)
    {
        if (!_transitions.TryGetValue(fromJob, out var rules))
        {
            return false;
        }
        
        var rule = rules.FirstOrDefault(r => r.ToJob == toJob);
        return rule != null && rule.CanTransition();
    }
    
    // 返回满足转换条件的所有状态
    public IEnumerable<StateTransition> GetPossibleTransition(JobBase fromJob)
    {
        if (!_transitions.TryGetValue(fromJob, out var rules))
        {
            return Enumerable.Empty<StateTransition>();
        }
        
        return rules.Union(_anyTransitions).Where(r => r.CanTransition());
    }

    public IEnumerable<JobBase> GetAllJobs()
    {
        var fromTransitions = _transitions.Values
            .SelectMany(transitions => transitions.Select(t => t.ToJob));
        
        var anyTransitions = _anyTransitions.Select(t => t.ToJob);
        
        return fromTransitions.Union(anyTransitions);
    }


    // 添加链式调用方法
    public StateTransitionContainer AddTransitionGroup(JobBase fromJob, IEnumerable<StateTransition> transitions)
    {
        foreach (var transition in transitions)
        {
            AddTransition(fromJob, transition.ToJob, transition.Condition, transition.Mode);
        }
        return this;
    }

}
