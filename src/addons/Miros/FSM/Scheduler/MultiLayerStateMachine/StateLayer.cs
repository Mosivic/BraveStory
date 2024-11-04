using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FSM.Job;
using FSM.States;
using Godot;

public class StateLayer
{
    public GameplayTag Layer { get; }
    private readonly AbsState _defaultState;
    private AbsState _currentState;
    private AbsState _lastState;
    private AbsState _delayState = null;

    private readonly StateTransitionContainer _transitionContainer;

    private readonly Dictionary<AbsState, IJob> _jobs;

    private GameplayTagContainer _ownedTags;

    private double _currentStateTime;
    
    public StateLayer(GameplayTag layerTag,AbsState defaultState,
        StateTransitionContainer transitionRuleContainer,
        Dictionary<AbsState, IJob> jobs,
        GameplayTagContainer ownedTags)
    {
        Layer = layerTag;
        _defaultState = defaultState;
        _currentState = defaultState;
        _transitionContainer = transitionRuleContainer;
        _jobs = jobs;
        _ownedTags = ownedTags;
    }

    public void Update(double delta)
    {
        ProcessNextState();
        
        _jobs[_currentState].Update(delta);
        _currentStateTime += delta;
    }
    
    public void PhysicsUpdate(double delta)
    {
        _jobs[_currentState].PhysicsUpdate(delta);
    }
    

    private void ProcessNextState()
    {
        if(_delayState != null)
        {
            if(_jobs[_currentState].CanExit())
            {
                TransformState(_delayState);
                _delayState = null;
                return;
            }

            return;
        }

        var transitions = _transitionContainer.GetPossibleTransition(_currentState);

        // 使用LINQ获取优先级最高且可进入的状态
        var nextTransition = transitions
            .OrderByDescending(t => t.ToState.Priority)
            .Where(t => t.Mode switch
            {
                StateTransitionMode.Normal => t.CanTransition() && _jobs[_currentState].CanExit() && _jobs[t.ToState].CanEnter(),
                StateTransitionMode.Force => t.CanTransition() && _jobs[t.ToState].CanEnter(),
                StateTransitionMode.Delay => t.CanTransition() && _jobs[t.ToState].CanEnter(),
                _ => throw new ArgumentException($"Unsupported transition mode: {t.Mode}")
            })
            .FirstOrDefault();
        
        if (nextTransition == null) return;
        
        if(nextTransition.Mode == StateTransitionMode.Delay)
        {
            _delayState = nextTransition.ToState;

            if(_jobs[_currentState].CanExit())
            {
                TransformState(nextTransition.ToState);
                _delayState = null;
            }
        }
        else
        {
            TransformState(nextTransition.ToState);
        }
    }

    private void TransformState(AbsState nextState)
    {
        var nextJob = _jobs[nextState];
        var currentJob = _jobs[_currentState];
        
        // 检查是否可以堆叠
        if (nextState.IsStack)
        {
            nextJob.Stack(nextState.Source);
        }
        
        currentJob.Exit();
        nextJob.Enter();
        
        _ownedTags.RemoveTag(_currentState.Tag);
        _ownedTags.AddTag(nextState.Tag);
        
        _lastState = _currentState;
        _currentState = nextState;
        _currentStateTime = 0.0;
        
#if DEBUG && true
        GD.Print($"[{Engine.GetProcessFrames()}] {_lastState.Name} -> {_currentState.Name}.");
#endif
    }

    public AbsState GetNowState()
    {
        return _currentState;
    }

    public AbsState GetLastState()
    {
        return _lastState;
    }

    public double GetCurrentStateTime()
    {
        return _currentStateTime;
    }

}