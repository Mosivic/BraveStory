using System.Collections.Generic;

using Miros.Core;

public class MultiLayerStateMachine:AbsScheduler, IScheduler
{
    private readonly Dictionary<Tag, StateLayer> _layers = new();
    private Dictionary<AbsState, IJob> _jobs = new();
    private TagContainer _ownedTags;
    
    
    public void SetOwnedTags(TagContainer ownedTags){
        _ownedTags = ownedTags;
    }

    
    public void AddLayer(Tag layer,AbsState defaultState,StateTransitionContainer transitionContainer){
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
        return job.State.Status == RunningStatus.Running;
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

    public AbsState GetNowState(Tag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetNowState();
        }
        return null;
    }

    public AbsState GetLastState(Tag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetLastState();
        }
        return null;
    }

    public double GetCurrentStateTime(Tag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetCurrentStateTime();
        }
        return 0;
    }
}