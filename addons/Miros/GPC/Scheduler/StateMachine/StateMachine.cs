using System;
using System.Collections.Generic;
using GPC.Job.Executor;
using GPC.States;

namespace GPC.Scheduler;

public class StateMachine(StateSet stateSet) : AbsScheduler(stateSet)
{
    private readonly HashSet<ITransition> _anyTransitions = new();
    private readonly Dictionary<string, StateNode> _nodes = new();
    private readonly JobExecutorProvider<StaticJobExecutor> _provider = new();
    private StateNode _current;

    public override void Update(double delta)
    {
        var transition = GetTransition();
        if (transition != null)
            ChangeState(transition.To);

        _provider.Executor.Update(_current.State, delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        _provider.Executor.PhysicsUpdate(_current.State, delta);
    }

    public void SetState(AbsState state)
    {
        _current = _nodes[state.Id];
        _provider.Executor.Enter(state);
    }

    private void ChangeState(AbsState state)
    {
        if (state.Equals(_current.State)) return;

        var previousState = _current.State;
        var nextState = _nodes[state.Id].State;

        _provider.Executor.Exit(previousState);
        _provider.Executor.Enter(nextState);
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

    public void AddTransition(CompoundState from, CompoundState to, Func<bool> conditionFunc)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, conditionFunc);
    }

    public void AddAnyTransition(CompoundState to, Func<bool> conditionFunc)
    {
        _anyTransitions.Add(new Transition(GetOrAddNode(to).State, conditionFunc));
    }

    private StateNode GetOrAddNode(CompoundState state)
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

internal class StateNode(CompoundState state)
{
    public readonly CompoundState State = state;

    public HashSet<ITransition> Transitions { get; } = new();

    public void AddTransition(CompoundState state, Func<bool> conditionFunc)
    {
        Transitions.Add(new Transition(state, conditionFunc));
    }
}