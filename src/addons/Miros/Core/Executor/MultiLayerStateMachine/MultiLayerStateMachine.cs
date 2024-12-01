using System.Collections.Generic;
using Miros.Core;

public class MultiLayerStateMachine : ExecutorBase<TaskBase>, IExecutor
{
    private readonly Dictionary<Tag, StateLayer> _layers = [];


    public void AddLayer(Tag layer, TaskBase defaultTask, StateTransitionContainer container)
    {
        _layers[layer] = new StateLayer(layer, defaultTask, container);
    }

    public override bool HasTaskRunning(ITask task)
    {
        var stateTask = task as TaskBase;
        return stateTask.IsActive;
    }

    public override void Update(double delta)
    {
        foreach (var key in _layers.Keys) _layers[key].Update(delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var key in _layers.Keys) _layers[key].PhysicsUpdate(delta);
    }

    public override TaskBase GetNowTask(Tag layer)
    {
        if (_layers.ContainsKey(layer)) return _layers[layer].GetNowTask();
        return null;
    }

    public override TaskBase GetLastTask(Tag layer)
    {
        if (_layers.ContainsKey(layer)) return _layers[layer].GetLastTask();
        return null;
    }

    public override double GetCurrentTaskTime(Tag layer)
    {
        if (_layers.ContainsKey(layer)) return _layers[layer].GetCurrentTaskTime();
        return 0;
    }
}