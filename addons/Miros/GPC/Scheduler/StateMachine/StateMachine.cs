using System.Collections.Generic;
using GPC.Job.Config;

namespace GPC.AI.StateMachine;

public class StateMachine : AbsScheduler
{
    private readonly HashSet<ITransition> _anyTransitions = new();
    private StateNode _current;
    private readonly Dictionary<string, StateNode> _nodes = new();

    public StateMachine(List<State> states) : base(states)
    {
    }

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

    public void SetState(State state)
    {
        _current = _nodes[state.Id];
        JobWrapper.Enter(state);
    }

    private void ChangeState(State state)
    {
        if (state == _current.State) return;

        var previousState = _current.State;
        var nextState = _nodes[state.Id].State;

        JobWrapper.Exit(previousState);
        JobWrapper.Enter(nextState);
        _current = _nodes[state.Id];
    }

    private ITransition GetTransition()
    {
        foreach (var transition in _anyTransitions)
            if (transition.Condition.IsSatisfy(_current.State))
                return transition;

        foreach (var transition in _current.Transitions)
            if (transition.Condition.IsSatisfy(_current.State))
                return transition;

        return null;
    }

    public void AddTransition(State from, State to, ICondition condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(State to, ICondition condition)
    {
        _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
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

internal class StateNode
{
    public State State;

    public StateNode(State state)
    {
        State = state;
        Transitions = new HashSet<ITransition>();
    }

    public HashSet<ITransition> Transitions { get; }

    public void AddTransition(State state, ICondition condition)
    {
        Transitions.Add(new Transition(state, condition));
    }
}