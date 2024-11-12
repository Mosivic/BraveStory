using System.Collections.Generic;

using Miros.Core;

public class MultiLayerStateMachine:AbsScheduler<JobBase>, IScheduler<JobBase>
{
    private readonly Dictionary<Tag, StateLayer> _layers = [];
    private HashSet<JobBase> _jobs = [];
    private TagContainer _ownedTags;
    
    
    public void SetOwnedTags(TagContainer ownedTags){
        _ownedTags = ownedTags;
    }


    public void AddLayer(Tag layer,JobBase defaultJob,StateTransitionContainer transitionContainer){
        _layers[layer] = new StateLayer(layer,defaultJob,transitionContainer);
    }

    public void AddJob(JobBase job)
    {
        _jobs.Add(job);
    }

    public void RemoveJob(JobBase job)
    {
        _jobs.Remove(job);
    }

    public bool HasJobRunning(JobBase job)
    {
        return job.Status == RunningStatus.Running;
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

    public JobBase GetNowJob(Tag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetNowJob();
        }
        return null;
    }

    public JobBase GetLastJob(Tag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetLastJob();
        }
        return null;
    }
    
    public double GetCurrentJobTime(Tag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetCurrentJobTime();
        }
        return 0;
    }


}