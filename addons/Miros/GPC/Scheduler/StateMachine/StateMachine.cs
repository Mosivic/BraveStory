﻿using System.Collections.Generic;
using Godot;
using GPC.State;

namespace GPC.Scheduler;

public class StateMachine(Node host,StateSpace stateSpace) : AbsScheduler(host,stateSpace)

{
    private readonly HashSet<ITransition> _anyTransitions = new();
    private readonly Dictionary<string, StateNode> _nodes = new();
    private StateNode _current;

    public override void Update(double delta)
    {
        var transition = GetTransition();
        if (transition != null)
            ChangeState(transition.To);

        JobWrapper.Update(_current.State, delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        JobWrapper.PhysicsUpdate(_current.State, delta);
    }

    public void SetState(AbsState state)
    {
        _current = _nodes[state.Id];
        JobWrapper.Enter(state);
    }

    private void ChangeState(AbsState state)
    {
        if (state.Equals(_current.State)) return;

        var previousState = _current.State;
        var nextState = _nodes[state.Id].State;

        JobWrapper.Exit(previousState);
        JobWrapper.Enter(nextState);
        _current = _nodes[state.Id];
    }

    private ITransition GetTransition()
    {
        foreach (var transition in _anyTransitions)
            if (transition.Condition.IsAllSatisfy(transition.To))
                return transition;

        foreach (var transition in _current.Transitions)
            if (transition.Condition.IsAllSatisfy(transition.To))
                return transition;

        return null;
    }

    public void AddTransition(AbsState from, AbsState to, Condition condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(AbsState to, Condition condition)
    {
        _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
    }

    private StateNode GetOrAddNode(AbsState state)
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

internal class StateNode(AbsState state)
{
    public readonly AbsState State = state;

    public HashSet<ITransition> Transitions { get; } = new();

    public void AddTransition(AbsState state, Condition condition)
    {
        Transitions.Add(new Transition(state, condition));
    }
}