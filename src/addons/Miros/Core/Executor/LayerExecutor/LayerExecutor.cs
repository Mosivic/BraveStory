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
    private State _defaultState;
    private State _lastState;
    private State _nextState;

    private TransitionMode _nextStateTransitionMode;


    public LayerExecutor(Tag layerTag,
        TransitionContainer transitionContainer, Dictionary<Tag, State> states)
    {
        Layer = layerTag;
        _transitionContainer = transitionContainer;
        _states = states;
    }

    public Tag Layer { get; }


    public void SetCurrentState(State state)
    {
        _currentState = state;
    }

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

        _currentState?.Task.Update(_currentState, delta);
    }

    public void PhysicsUpdate(double delta)
    {
        _currentState?.Task.PhysicsUpdate(_currentState, delta);
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
        SearchNextState();
    }

    private void SearchNextState()
    {
        // 如果当前状态是固定模式，并且状态为运行中，则不进行状态转换
        if (_currentState.Status == RunningStatus.Running && _currentState.InterruptPolicy == InterruptPolicy.Fixed)
            return;


        var transitions = _transitionContainer.GetPossibleTransition(_currentState);

        // 使用LINQ获取优先级最高且可进入的状态
        var sortedTransitions = transitions
            .OrderByDescending(t => t.Priority)
            .ToArray();

        if (sortedTransitions.Length == 0) return; // 没有可转换的状态

        foreach (var t in sortedTransitions)
        {
            var maybeState = _states[t.To];

            switch (t.Mode)
            {
                // 正常模式, 切换需要满足：1. 当前任务可以退出 2. 新任务可以进入 3.切换条件为真
                case TransitionMode.Normal: 
                    if (t.CanTransition() && _currentState.Task.CanExit(_currentState) &&
                        maybeState.Task.CanEnter(maybeState))
                    {
                        _nextState = maybeState;
                        _nextStateTransitionMode = t.Mode;
                        return;
                    }
                    break;
                // 强制模式, 切换需要满足：1. 切换条件为真
                case TransitionMode.Force:
                    if (t.CanTransition())
                    {
                        _nextState = maybeState;
                        _nextStateTransitionMode = t.Mode;
                        return;
                    }
                    break;
                // 延迟前模式, 切换需要满足：1. 切换条件为真 2. 新任务可以进入
                case TransitionMode.DelayFront:
                    if (t.CanTransition() && maybeState.Task.CanEnter(maybeState))
                    {
                        _nextState = maybeState;
                        _nextStateTransitionMode = t.Mode;
                        return;
                    }
                    break;
                // 延迟后模式, 切换需要满足：1. 切换条件为真 2. 当前任务可以退出
                case TransitionMode.DelayBackend:
                    if (t.CanTransition() && _currentState.Task.CanExit(_currentState))
                    {
                        _nextState = maybeState;
                        _nextStateTransitionMode = t.Mode;
                        return;
                    }
                    break;
            }
        }
    }

    private void SwitchNextState()
    {
        if (_nextStateTransitionMode == TransitionMode.DelayFront &&
            !_currentState.Task.CanExit(_currentState)) // 等待当前任务满足退出条件
            return;

        if (_nextStateTransitionMode == TransitionMode.DelayBackend &&
            !_nextState.Task.CanEnter(_nextState)) // 等待新任务满足进入条件
            return;

        _lastState = _currentState;

        if (_currentState.InterruptPolicy == InterruptPolicy.Fallback) // 回退模式
        {
            _currentState.Task.Pause(_currentState);

            if(_nextState.Status == RunningStatus.None)
                _nextState.Task.Enter(_nextState);
            else if(_nextState.Status == RunningStatus.Paused)
                _nextState.Task.Resume(_nextState);
            

            _currentState = _nextState;
            _nextState = _lastState;
        }
        else
        {
            _currentState.Task.Exit(_currentState);
            
            if(_nextState.Status == RunningStatus.None)
                _nextState.Task.Enter(_nextState);
            else if(_nextState.Status == RunningStatus.Paused)
                _nextState.Task.Resume(_nextState);

            _currentState = _nextState;
            _nextState = null;
        }
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