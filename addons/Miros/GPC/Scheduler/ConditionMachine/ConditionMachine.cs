using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GPC.Job.Executor;
using GPC.States;

namespace GPC.Scheduler;

public class ConditionMachine : AbsScheduler
{
    private readonly JobExecutorProvider<StaticJobExecutor> _provider = new();
    private readonly Dictionary<Layer, List<AbsState>> _runningStates = new();
    
    public ConditionMachine(List<AbsState> states)
    {
        foreach (var state in states)
        {
            AddState(state);
        }
        
    }
    
    public void AddState(AbsState state) 
    {
        var layer = state.Layer;
        if (!States.ContainsKey(layer))
        {
            States.TryAdd(layer, []);
            _runningStates.TryAdd(layer, []);
        }
        
        var states = States[layer];
        var result = states.FindIndex((s => s.Name == state.Name));
        
        if (result == -1)
        {
            states.Add(state);
            return;
        }
        // State Stack
        var livedState = states[result];
        
        if (!livedState.IsStack) states.Add(state);
        else _provider.Executor.Stack(livedState,state);
    }

    public void RemoveState(AbsState state)
    {
        
    }
    
    public override void Update(double delta)
    {
        UpdateRunningStates();
        foreach (var state in _runningStates.Values.SelectMany(states => states.Where(state => state != null)))
            _provider.Executor.Update(state, delta);
    }

    
    public override void PhysicsUpdate(double delta)
    {
        foreach (var state in _runningStates.Values.SelectMany(states => states.Where(state => state != null)))
            _provider.Executor.PhysicsUpdate(state, delta);
    }
    
    
    
    private void UpdateRunningStates()
    {
        foreach (var layer in _runningStates.Keys)
        {
            RemoveRunOverState(layer);
            
            if (layer.JobMaxCount < 1) continue;

            var nextState = GetBestState(layer);
            if (nextState != null) 
                AddPreparedState(nextState);
        }
    }
    
    
    private void RemoveRunOverState(Layer layer)
    {
        var layerStates = _runningStates[layer];
        var layerStateCount = layerStates.Count();
        if (layerStateCount == 0) return;
        
        for (var i = 0; i < layerStateCount; i++)
        {
            var state = layerStates[i];
            if (state.Status is JobRunningStatus.Succeed)
            {
                StateChanged.Invoke(state);
                _provider.Executor.Over(state);
                layerStates.RemoveAt(i);
            }
            else if (state.Status is JobRunningStatus.Failed)
            {
                StateChanged.Invoke(state);
                layerStates.RemoveAt(i);
            }
        }
    }
    
    
    private void AddPreparedState(AbsState state)
    {
        var layer = state.Layer;
        var layerStates = _runningStates[layer];
        var layerStateCount = layerStates.Count();
        
        //考虑加入NextState
        //当前层无State
        if (layerStateCount == 0)
        {
            StateChanged.Invoke(state);
            _provider.Executor.Start(state);
            layerStates.Add(state);
        }
        //当前层有未满，可加入NextState
        else if (layerStateCount < layer.JobMaxCount)
        {
            StateChanged.Invoke(state);
            _provider.Executor.Start(state);

            var insertIndex = layerStates.FindIndex(s => state.Priority < s.Priority);
            if (insertIndex == -1)
                layerStates.Add(state);
            else
                layerStates.Insert(insertIndex, state);
        }
        //当前层已满，加入高优先State
        else if (layerStateCount == layer.JobMaxCount && state.Priority > layerStates[0].Priority)
        {
            StateChanged.Invoke(layerStates[0]);
            _provider.Executor.Over(layerStates[0]);
            layerStates.RemoveAt(0);

            StateChanged.Invoke(state);
            _provider.Executor.Start(state);

            var insertIndex = layerStates.FindIndex(s => state.Priority < s.Priority);
            if (insertIndex == -1)
                layerStates.Add(state);
            else
                layerStates.Insert(insertIndex, state);
        }
    }

    
    private AbsState GetBestState(Layer layer)
    {
        var states = new List<AbsState>();

        foreach (var state in States[layer])
        {
            if (_provider.Executor.IsPrepared(state))
            {
                StatePrepared.Invoke(state);
                states.Add(state);
            }
        }
        
        if (states.Count == 0)
            return null;
        return GetHighestCfg(states);
    }


    private static AbsState GetHighestCfg(List<AbsState> states)
    {
        if (states.Count == 0) return null;
        AbsState bestState = null;
        foreach (var state in states)
        {
            bestState ??= state;
            if (state.Priority > bestState.Priority) bestState = state;
        }

        return bestState;
    }
}