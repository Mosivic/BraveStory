using System.Collections.Generic;
using Godot;
using GPC.Job;
using GPC.Job.Executor;
using GPC.States;

namespace GPC.Scheduler;

public class ConditionMachine : AbsScheduler 
{
    private readonly Dictionary<Layer, AbsState> _jobsExecute = new();
    private readonly JobExecutorProvider<StaticJobExecutor> _provider = new();
    
    public ConditionMachine(StateSet stateSet) : base(stateSet)
    {
        foreach (var state in stateSet.States)
        {
            var layer = state.Layer;
            _jobsExecute.TryAdd(layer, null);
        }
    }

    public override void Update(double delta)
    {
        _UpdateJob();
        foreach (var state in _jobsExecute.Values)
            if (state != null)
                _provider.Executor.Update(state, delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var state in _jobsExecute.Values)
            if (state != null)
                _provider.Executor.PhysicsUpdate(state, delta);
    }

    private void _UpdateJob()
    {
        
        foreach (var layer in _jobsExecute.Keys)
        {
            var currentState = _jobsExecute[layer];
            var nextState = _GetBestState(layer);

            if (currentState == null)
            {
                if (nextState == null) return;

                _jobsExecute[layer] = nextState;
                _provider.Executor.Enter(nextState);
                
                 StateChanged.Invoke(nextState);
            }
            else
            {
                if (currentState.RunningStatus == JobRunningStatus.NoRunning)
                {
                    if (nextState == null)
                    {
                        _provider.Executor.Exit(currentState);
                        _jobsExecute[layer] = null;
                        
                        StateChanged.Invoke(currentState);
                    }
                    else
                    {
                        _provider.Executor.Exit(currentState);
                        _jobsExecute[layer] = nextState;
                        _provider.Executor.Enter(nextState);
                        
                        StateChanged.Invoke(currentState);
                        StateChanged.Invoke(nextState);
                    }
                }
                else
                {
                    if (nextState == null) return;

                    if (nextState.Priority > currentState.Priority)
                    {
                        _provider.Executor.Exit(currentState);
                        _jobsExecute[layer] = nextState;
                        _provider.Executor.Enter(nextState);
                        
                        StateChanged.Invoke(currentState);
                        StateChanged.Invoke(nextState);
                    }
                }
            }
        }
    }

    private AbsState _GetBestState(Layer layer)
    {
        List<AbsState> candicateJobsCfg = [];
        foreach (var state in StateSet.States)
        {
            if (state.Layer != layer) continue;
            if(_provider.Executor.IsPrepared(state))
                candicateJobsCfg.Add(state);
        }

        if (candicateJobsCfg.Count == 0)
            return null;
        return GetHighestCfg(candicateJobsCfg);
        
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