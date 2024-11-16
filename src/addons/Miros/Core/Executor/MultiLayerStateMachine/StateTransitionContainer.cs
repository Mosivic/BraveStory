using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class StateTransitionContainer
{
    private readonly HashSet<StateTransition> _anyTransitions = [];
    private readonly Dictionary<TaskBase, HashSet<StateTransition>> _transitions = [];


    public StateTransitionContainer Add(TaskBase fromTask, StateTransition transition)
    {
        if (!_transitions.ContainsKey(fromTask))
            _transitions[fromTask] = [];
        _transitions[fromTask].Add(transition);
        return this;
    }

    public StateTransitionContainer AddAny(StateTransition transition)
    {
        _anyTransitions.Add(transition);
        return this;
    }

    // 返回满足转换条件的所有状态
    public IEnumerable<StateTransition> GetPossibleTransition(TaskBase fromTask)
    {
        if (!_transitions.TryGetValue(fromTask, out var rules)) return Enumerable.Empty<StateTransition>();

        return rules.Union(_anyTransitions).Where(r => r.CanTransition());
    }
}