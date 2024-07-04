using System;
using System.Collections.Generic;
using Godot;
using GPC.States;

namespace GPC.Scheduler;

public class StateMachine(Node host, StateSet stateSet) : AbsScheduler(stateSet)

{
    private readonly HashSet<ITransition> _anyTransitions = new();
    private readonly Dictionary<string, StateNode> _nodes = new();
    private StateNode _current;

    public override void Update(double delta)
    {
        var transition = GetTransition();
        if (transition != null)
            ChangeState(transition.To);

        _jobWrapper.Update(_current.State, delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        _jobWrapper.PhysicsUpdate(_current.State, delta);
    }

    public void SetState(AbsState state)
    {
        _current = _nodes[state.Id];
        _jobWrapper.Enter(state);
    }

    private void ChangeState(AbsState state)
    {
        if (state.Equals(_current.State)) return;

        var previousState = _current.State;
        var nextState = _nodes[state.Id].State;

        _jobWrapper.Exit(previousState);
        _jobWrapper.Enter(nextState);
        _current = _nodes[state.Id];
    }

    private ITransition GetTransition()
    {
        foreach (var transition in _anyTransitions)
            if (transition.ConditionFunc.Invoke())
                return transition;

        foreach (var transition in _current.Transitions)
            if (transition.ConditionFunc.Invoke())
                return transition;

        return null;
    }

    public void AddTransition(State from, State to, Func<bool> conditionFunc)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, conditionFunc);
    }

    public void AddAnyTransition(State to, Func<bool> conditionFunc)
    {
        _anyTransitions.Add(new Transition(GetOrAddNode(to).State, conditionFunc));
    }

    private StateNode GetOrAddNode(State state)
    {
        var node = _nodes.GetValueOrDefault(state.Id);
        if (node == null)
        {
            node = new StateNode(state);
            _nodes.Add(state.Id, node);
        }

        return node;
    }
}

internal class StateNode(State state)
{
    public readonly State State = state;

    public HashSet<ITransition> Transitions { get; } = new();

    public void AddTransition(State state, Func<bool> conditionFunc)
    {
        Transitions.Add(new Transition(state, conditionFunc));
    }
}