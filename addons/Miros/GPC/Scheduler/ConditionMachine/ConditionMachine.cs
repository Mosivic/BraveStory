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
    
    public ConditionMachine(List<AbsState> states)
    {
        foreach (var state in states)
        {
            AddLayerState(state);
        }
        
    }
    
    public void AddLayerState(AbsState state)
    {
        var layer = state.Layer;
        if (!LayerStates.ContainsKey(layer))
        {
            LayerStates.TryAdd(layer, []);
            RunningLayerStates.TryAdd(layer, []);
        }
        
        LayerStates[layer].Add(state);
    }


    public void AddRunningLayerState(AbsState state)
    {
        var layer = state.Layer;
        if (!RunningLayerStates.ContainsKey(layer)){}
            RunningLayerStates.TryAdd(layer, []);
        
        //CurrentLayer no state
        if (RunningLayerStates[layer].Count == 0)
        {
            RunningLayerStates[layer].Add(state);
            return;
        }
        
        //State can't stack
        var index = RunningLayerStates[layer].FindIndex((s => s.Name == state.Name));
        if (!state.IsStack)
        {
            if (index == -1)
                RunningLayerStates[layer].Add(state);
            return;
        }
        
        //Find the same state AND Judge to stack
        if (index == -1)
        {
            RunningLayerStates[layer].Add(state);
        }
        else
        {
            var insertIndex = RunningLayerStates[layer].FindIndex(s => state.Priority < s.Priority);
            if (insertIndex == -1)
                RunningLayerStates[layer].Add(state);
            else
                RunningLayerStates[layer].Insert(insertIndex, state);
            _provider.Executor.Stack(RunningLayerStates[layer][index],state);
        }
        
    }
    
    public void RemoveState(AbsState state)
    {
        
    }
    
    public override void Update(double delta)
    {
        UpdateRunningStates();
        foreach (var state in RunningLayerStates.Values.SelectMany(states => states.Where(state => state != null)))
            _provider.Executor.Update(state, delta);
    }

    
    public override void PhysicsUpdate(double delta)
    {
        foreach (var state in RunningLayerStates.Values.SelectMany(states => states.Where(state => state != null)))
            _provider.Executor.PhysicsUpdate(state, delta);
    }
    
    
    
    private void UpdateRunningStates()
    {
        foreach (var layer in LayerStates.Keys)
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
        var layerStates = RunningLayerStates[layer];
        if (layerStates.Count == 0) return;
        
        for (var i = 0; i < layerStates.Count ; i++)
        {
            var state = layerStates[i];
            if (state.Status is JobRunningStatus.Succeed or JobRunningStatus.Failed)
            {
                StateChanged.Invoke(state);
                _provider.Executor.Exit(state);
                layerStates.RemoveAt(i);
            }
        }
    }
    
    
    private void AddPreparedState(AbsState state)
    {
        var layer = state.Layer;
        var layerStates = RunningLayerStates[layer];
        var layerStateCount = layerStates.Count();
        
        //考虑加入NextState
        //当前层无State
        if (layerStateCount == 0)
        {
            StateChanged.Invoke(state);
            _provider.Executor.Enter(state);
            AddRunningLayerState(state);
        }
        //当前层有未满，可加入NextState
        else if (layerStateCount < layer.JobMaxCount)
        {
            StateChanged.Invoke(state);
            _provider.Executor.Enter(state);
            AddRunningLayerState(state);
        }
        //当前层已满，加入高优先State
        else if (layerStateCount == layer.JobMaxCount && state.Priority > layerStates[0].Priority)
        {
            StateChanged.Invoke(layerStates[0]);
            _provider.Executor.Exit(layerStates[0]);
            layerStates.RemoveAt(0);

            StateChanged.Invoke(state);
            _provider.Executor.Enter(state);
            AddRunningLayerState(state);
        }
    }

    
    private AbsState GetBestState(Layer layer)
    {
        var states = new List<AbsState>();

        foreach (var state in LayerStates[layer])
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