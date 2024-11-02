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
    private readonly StateTransitionContainer _transitionContainer;
    private readonly Dictionary<AbsState, IJob> _jobs = new();
    
    public StateLayer(GameplayTag layerTag,AbsState defaultState,StateTransitionContainer transitionRuleContainer,Dictionary<AbsState, IJob> jobs)
    {
        Layer = layerTag;
        _defaultState = defaultState;
        _currentState = defaultState;
        _transitionContainer = transitionRuleContainer;
        _jobs = jobs;
    }

    public void Update(double delta)
    {
        ProcessNextState();
        
        _jobs[_currentState].Update(delta);
        
    }
    
    public void PhysicsUpdate(double delta)
    {
        _jobs[_currentState].PhysicsUpdate(delta);
    }
    

    private void ProcessNextState()
    {
        var toStates = _transitionContainer.GetToStates(_currentState);

        // 使用LINQ获取优先级最高且可进入的状态
        var nextState = toStates
        .OrderByDescending(s => s.Priority)
        .Where(s => _jobs[s].CanEnter())
        .FirstOrDefault();
        
        if (nextState == null) return;
        
        var nextJob = _jobs[nextState];
        var currentJob = _jobs[_currentState];
        

        // 检查是否可以堆叠
        if (nextState.IsStack)
        {
            nextJob.Stack(nextState.Source);
        }
        
        currentJob.Exit();
        nextJob.Enter();
        
        
        _lastState = _currentState;
        _currentState = nextState;

        GD.Print($"[StateLayer] Change state to {_currentState.Name}.");
    }

    public AbsState GetCurrentState()
    {
        return _currentState;
    }
}