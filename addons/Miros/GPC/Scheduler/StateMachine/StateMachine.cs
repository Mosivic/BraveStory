using System.Collections.Generic;
using GPC.Job.Config;

namespace GPC.AI.StateMachine;

public class StateMachine<T>(List<T> states) : AbsScheduler<T>(states)
    where T : class, IState
{
    private readonly HashSet<ITransition<T>> _anyTransitions = new();
    private readonly Dictionary<string, StateNode<T>> _nodes = new();
    private StateNode<T> _current;

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

    public void SetState(T state)
    {
        _current = _nodes[state.Id];
        JobWrapper.Enter(state);
    }

    private void ChangeState(T state)
    {
        if (state.Equals(_current.State)) return;

        var previousState = _current.State;
        var nextState = _nodes[state.Id].State;

        JobWrapper.Exit(previousState);
        JobWrapper.Enter(nextState);
        _current = _nodes[state.Id];
    }

    private ITransition<T> GetTransition()
    {
        foreach (var transition in _anyTransitions)
            if (transition.Condition.IsAllSatisfy(transition.To))
                return transition;

        foreach (var transition in _current.Transitions)
            if (transition.Condition.IsAllSatisfy(transition.To))
                return transition;

        return null;
    }

    public void AddTransition(T from, T to, Condition condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(T to, Condition condition)
    {
        _anyTransitions.Add(new Transition<T>(GetOrAddNode(to).State, condition));
    }

    private StateNode<T> GetOrAddNode(T state)
    {
        var node = _nodes.GetValueOrDefault(state.Id);
        if (node == null)
        {
            node = new StateNode<T>(state);
            _nodes.Add(state.Id, node);
        }

        return node;
    }
}

internal class StateNode<T>(T state)
    where T : IState
{
    public readonly T State = state;

    public HashSet<ITransition<T>> Transitions { get; } = new();

    public void AddTransition(T state, Condition condition)
    {
        Transitions.Add(new Transition<T>(state, condition));
    }
}