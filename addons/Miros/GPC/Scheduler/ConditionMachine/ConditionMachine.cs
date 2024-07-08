using System.Collections.Generic;
using System.Linq;
using GPC.Job.Executor;
using GPC.States;

namespace GPC.Scheduler;

public class ConditionMachine : AbsScheduler
{
    private readonly Dictionary<Layer, List<AbsState>> _runningStates = new();
    private readonly JobExecutorProvider<StaticJobExecutor> _provider = new();
    
    public ConditionMachine(StateSet stateSet) : base(stateSet)
    {
        foreach (var layer in stateSet.States.Select(state => state.Layer))
        {
            _runningStates[layer] = [];
        }
    }

    public override void Update(double delta)
    {
        _UpdateState();
        foreach (var state in _runningStates.Values.SelectMany(states => states.Where(state => state != null)))
            _provider.Executor.Update(state, delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var state in _runningStates.Values.SelectMany(states => states.Where(state => state != null)))
            _provider.Executor.PhysicsUpdate(state, delta);
    }

    private void _UpdateState()
    {
        foreach (var layer in _runningStates.Keys)
        {
            var layerStates = _runningStates[layer];
            
            var layerStateMaxCount = layer.JobMaxCount;
            var layerStateCount = layerStates.Count();
            
            //结束运行完成的State
            if (layerStateCount != 0) 
            {
                for (int i = 0; i < layerStateCount; i++) 
                {
                    var layerJob = layerStates[i];
                    if (!layerJob.IsRunning)
                    {
                        StateChanged.Invoke(layerJob,JobRunningStatus.Exit);
                        _provider.Executor.Exit(layerJob);
                        layerStates.RemoveAt(i);
                    }
                }
                layerStateCount = layerStates.Count();
            }
            
            if(layerStateMaxCount < 1) continue;
            
            var nextState = _GetBestState(layer);
            if (nextState == null) continue;
            
            //考虑加入NextState
            //当前层无State
            if (layerStateCount == 0) 
            {
                StateChanged.Invoke(nextState,JobRunningStatus.Enter);
                _provider.Executor.Enter(nextState);
                layerStates.Add(nextState);
            }
            //当前层有未满，可加入NextState
            else if (layerStateCount < layerStateMaxCount)
            {
                StateChanged.Invoke(nextState, JobRunningStatus.Enter);
                _provider.Executor.Enter(nextState);
                
                var insertIndex = layerStates.FindIndex(state =>nextState.Priority < state.Priority );
                if(insertIndex == -1)
                    layerStates.Add(nextState);
                else
                    layerStates.Insert(insertIndex,nextState);
           
            }
            //当前层已满，加入高优先State
            else if(layerStateCount == layerStateMaxCount && nextState.Priority > layerStates[0].Priority) 
            {
                StateChanged.Invoke(layerStates[0],JobRunningStatus.Exit);
                _provider.Executor.Exit(layerStates[0]);
                layerStates.RemoveAt(0);
                
                StateChanged.Invoke(nextState,JobRunningStatus.Enter);
                _provider.Executor.Enter(nextState);
                
                var insertIndex = layerStates.FindIndex(state =>nextState.Priority < state.Priority );
                if(insertIndex == -1)
                    layerStates.Add(nextState);
                else
                    layerStates.Insert(insertIndex,nextState);
            }
        }
    }

    private AbsState _GetBestState(Layer layer)
    {
        var states = new List<AbsState>();
        
        foreach (var state in StateSet.States.Where(state => state.Layer == layer).Where(state => _provider.Executor.IsPrepared(state)))
        {
            StatePrepared.Invoke(state);
            states.Add(state);
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