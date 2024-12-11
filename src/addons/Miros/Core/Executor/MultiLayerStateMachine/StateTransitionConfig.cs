using System;
using System.Collections.Generic;

namespace Miros.Core;

// 状态转移配置
// 用于配置状态转移规则
public class StateTransitionConfig
{
    private readonly List<Transition> _anyTransitions = [];

    public IEnumerable<Transition> AnyTransitions => _anyTransitions;
    public Dictionary<State, List<Transition>> Transitions { get; } = [];


    public StateTransitionConfig AddAny(State toState, Func<bool> condition = null,
        TransitionMode mode = TransitionMode.Normal)
    {
        _anyTransitions.Add(new Transition(null, toState, condition, mode));
        return this;
    }

    public StateTransitionConfig Add(State fromState, State toState, Func<bool> condition = null,
        TransitionMode mode = TransitionMode.Normal)
    {
        if (!Transitions.ContainsKey(fromState))
            Transitions[fromState] = [];
        Transitions[fromState].Add(new Transition(fromState, toState, condition, mode));
        return this;
    }

    public record Transition(State FromState, State ToState, Func<bool> Condition, TransitionMode Mode);
}