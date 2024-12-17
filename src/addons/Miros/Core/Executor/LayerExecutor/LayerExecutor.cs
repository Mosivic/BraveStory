using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;


// FIXME: 修复状态转换的逻辑
public class LayerExecutor
{
    private readonly Dictionary<Tag, State> _states = [];
    private readonly TransitionContainer _transitionContainer;

    private State _currentState;
    private State _lastState;
    private State _nextState;
    private State _defaultState;

    private TransitionMode _nextStateTransitionMode;


    public LayerExecutor(Tag layerTag,
        TransitionContainer transitionContainer, Dictionary<Tag, State> states)
    {
        Layer = layerTag;
        _transitionContainer = transitionContainer;
        _states = states;
    }

    public Tag Layer { get; }


    public void SetDefaultState(State state)
    {
        _defaultState = state;
    }

    public void SetNextState(State state, TransitionMode mode)
    {
        _nextState = state;
        _nextStateTransitionMode = mode;
    }

    public void Update(double delta)
    {
        ProcessNextState();

        _currentState.Task.Update(_currentState,delta);
    }

    public void PhysicsUpdate(double delta)
    {
        _currentState.Task.PhysicsUpdate(_currentState,delta);
    }


    private void ProcessNextState()
    {
        if (_currentState == null)
        {
            _currentState = _defaultState;
            return;
        }

        if (_nextState != null)
        {
            SwitchNextState();
            return;
        }


        // 当没有下一个任务时（_nextTask == null），检查当前任务是否有可转换的状态
        var transitions = _transitionContainer.GetPossibleTransition(_currentState);

        // 使用LINQ获取优先级最高且可进入的状态
        var sortedTransitions = transitions
            .OrderByDescending(t => t.Priority)
            .ToArray();

        if (sortedTransitions.Length == 0) return; // 没有可转换的状态

        foreach (var t in sortedTransitions)
        {
            var state = _states[t.To];
            if (t.Mode switch
                {
                    TransitionMode.Normal => t.CanTransition() && _currentState.Task.CanExit(_currentState) && state.Task.CanEnter(state),
                    TransitionMode.Force => t.CanTransition(),
                    TransitionMode.DelayFront => t.CanTransition() && state.Task.CanEnter(state),
                    TransitionMode.DelayBackend => t.CanTransition() && _currentState.Task.CanExit(_currentState),
                    _ => throw new ArgumentException($"Unsupported transition mode: {t.Mode}")
                })
            {
                _nextState = state;
                _nextStateTransitionMode = t.Mode;
                return;
            }
        }
    }

    private void SwitchNextState()
    {
        if (_nextStateTransitionMode == TransitionMode.DelayFront && !_currentState.Task.CanExit(_currentState)) // 等待当前任务满足退出条件
            return;

        if (_nextStateTransitionMode == TransitionMode.DelayBackend && !_nextState.Task.CanEnter(_nextState)) // 等待新任务满足进入条件
            return;

        _currentState.Task.Exit(_currentState);
        _nextState.Task.Enter(_nextState);

        _lastState = _currentState;
        _currentState = _nextState;
        _nextState = null;
    }


    public State GetNowState()
    {
        return _currentState;
    }

    public State GetLastState()
    {
        return _lastState;
    }
}