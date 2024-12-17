using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class TransitionContainer
{
    private readonly List<Transition> _anyTransitions = [];
    private readonly Dictionary<State, List<Transition>> _transitions = [];


    public TransitionContainer AddTransitions(State fromState, Transition[] transitions)
    {
        if (!_transitions.ContainsKey(fromState))
            _transitions[fromState] = [];

        foreach (var transition in transitions)
            if (transition.IsAny)
                _anyTransitions.Add(transition);
            else
                _transitions[fromState].Add(transition);
        return this;
    }

    public void RemoveTransitions(State state)
    {
        _transitions.Remove(state);
    }

    public void RemoveAnyTransition(State state)
    {
        _anyTransitions.RemoveAll(t => t.To == state.Tag);
    }

    // 返回满足转换条件的所有状态
    public IEnumerable<Transition> GetPossibleTransition(State fromState)
    {
        if (!_transitions.TryGetValue(fromState, out var rules)) return Enumerable.Empty<Transition>();
        var ts = rules.Union(_anyTransitions);
        return ts.Where(r => r.To != fromState.Tag && r.CanTransition()); // 排除自身
    }
}