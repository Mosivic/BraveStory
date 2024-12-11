using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class TransitionContainer
{
    private readonly List<Transition> _anyTransitions = [];
    private readonly Dictionary<TaskBase, List<Transition>> _transitions = [];


    public TransitionContainer AddTransitions(TaskBase fromTask, Transition[] transitions)
    {
        if (!_transitions.ContainsKey(fromTask))
            _transitions[fromTask] = [];
        _transitions[fromTask].AddRange(transitions);
        return this;
    }

    public TransitionContainer AddAnyTransition(Transition transition)
    {
        _anyTransitions.Add(transition);
        return this;
    }

    public void RemoveTransitions(TaskBase task)
    {
        _transitions.Remove(task);
    }

    public void RemoveAnyTransition(TaskBase task)
    {
        _anyTransitions.RemoveAll(t => t.To == task.Tag);
    }

    // 返回满足转换条件的所有状态
    public IEnumerable<Transition> GetPossibleTransition(TaskBase fromTask)
    {
        if (!_transitions.TryGetValue(fromTask, out var rules)) return Enumerable.Empty<Transition>();

        return rules.Union(_anyTransitions).Where(r => r.CanTransition());
    }
}