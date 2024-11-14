using System.Collections.Generic;
using Miros.Core;

public class MultiLayerStateMachine : SchedulerBase<JobBase>
{
    private readonly Dictionary<Tag, StateLayer> _layers = [];


    public void AddLayer(Tag layer, Tag defaultJobSign, StateTransitionContainer transitionContainer)
    {
        _layers[layer] = new StateLayer(layer, defaultJobSign, transitionContainer, _jobs);
    }

    public override bool HasJobRunning(JobBase job)
    {
        return job.IsActive;
    }

    public override void Update(double delta)
    {
        foreach (var key in _layers.Keys) _layers[key].Update(delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var key in _layers.Keys) _layers[key].PhysicsUpdate(delta);
    }

    public override JobBase GetNowJob(Tag layer)
    {
        if (_layers.ContainsKey(layer)) return _layers[layer].GetNowJob();
        return null;
    }

    public override JobBase GetLastJob(Tag layer)
    {
        if (_layers.ContainsKey(layer)) return _layers[layer].GetLastJob();
        return null;
    }

    public override double GetCurrentJobTime(Tag layer)
    {
        if (_layers.ContainsKey(layer)) return _layers[layer].GetCurrentJobTime();
        return 0;
    }
}