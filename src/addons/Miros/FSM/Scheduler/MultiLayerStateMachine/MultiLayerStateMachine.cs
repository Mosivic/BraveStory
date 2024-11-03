using System;
using System.Collections.Generic;
using System.Linq;
using FSM.Job;
using FSM.Scheduler;
using FSM.States;
using Godot;

public class MultiLayerStateMachine:AbsScheduler, IScheduler
{
    private readonly Dictionary<GameplayTag, StateLayer> _layers = new();
    private Dictionary<AbsState, IJob> _jobs = new();
    private GameplayTagContainer _ownedTags;
    
    
    public void SetOwnedTags(GameplayTagContainer ownedTags){
        _ownedTags = ownedTags;
    }

    
    public void AddLayer(GameplayTag layer,AbsState defaultState,StateTransitionContainer transitionContainer){
        _layers[layer] = new StateLayer(layer,defaultState,transitionContainer,_jobs,_ownedTags);
    }

    public void AddJob(IJob job)
    {
        _jobs.TryAdd(job.State,job);
    }

    public void RemoveJob(IJob job)
    {
        _jobs.Remove(job.State);
    }

    public bool HasJobRunning(IJob job)
    {
        return job.State.Status == FSM.RunningStatus.Running;
    }

    public void Update(double delta)
    {
        foreach (var key in _layers.Keys)
        {
            _layers[key].Update(delta);
        }
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var key in _layers.Keys)
        {
            _layers[key].PhysicsUpdate(delta);
        }
    }

    public AbsState GetNowState(GameplayTag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetNowState();
        }
        return null;
    }

    public AbsState GetLastState(GameplayTag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetLastState();
        }
        return null;
    }
}