using System.Collections.Generic;

using Miros.Core;

public class MultiLayerStateMachine:AbsScheduler<NativeJob>, IScheduler<NativeJob>
{
    private readonly Dictionary<Tag, StateLayer> _layers = [];
    private HashSet<NativeJob> _jobs = [];
    private TagContainer _ownedTags;
    
    
    public void SetOwnedTags(TagContainer ownedTags){
        _ownedTags = ownedTags;
    }


    public void AddLayer(Tag layer,NativeJob defaultJob,StateTransitionContainer transitionContainer){
        _layers[layer] = new StateLayer(layer,defaultJob,transitionContainer);
    }

    public void AddJob(NativeJob job)
    {
        _jobs.Add(job);
    }

    public void RemoveJob(NativeJob job)
    {
        _jobs.Remove(job);
    }

    public bool HasJobRunning(NativeJob job)
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

    public NativeJob GetNowJob(Tag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetNowJob();
        }
        return null;
    }

    public NativeJob GetLastJob(Tag layer)
    {
        if(_layers.ContainsKey(layer)){
            return _layers[layer].GetLastJob();
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