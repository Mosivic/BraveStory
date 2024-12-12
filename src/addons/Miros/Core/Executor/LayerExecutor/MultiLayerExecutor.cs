using System;
using System.Collections.Generic;

namespace Miros.Core;

public class MultiLayerExecutor : ExecutorBase<TaskBase>, IExecutor
{
    private readonly Dictionary<Tag, LayerExecutor> _layers = [];
    private readonly TransitionContainer _transitionContainer = new();

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

    public override double GetCurrentTaskTime(Tag layer)
    {
        if (_layers.ContainsKey(layer)) return _layers[layer].GetCurrentTaskTime();
        return 0;
    }


    public override void AddTask(ITask task, Context context)
    {
        if (context is not MultiLayerExecutorContext fsmContext)
            throw new Exception("Invalid arguments for FSM ");

        var stateTask = task as TaskBase;
        base.AddTask(stateTask, context);

        if (!_layers.ContainsKey(fsmContext.Layer))
        {
            _layers[fsmContext.Layer] = new LayerExecutor(fsmContext.Layer, _transitionContainer, _tasks);
            _layers[fsmContext.Layer].SetDefaultTask(stateTask); //将第一个任务设置为默认任务
        }

        if (fsmContext.Transitions != null)
            _transitionContainer.AddTransitions(stateTask, fsmContext.Transitions);
    }

    public void RemoveTask(TaskBase task)
    {
        _tasks.Remove(task.Tag);
        _transitionContainer.RemoveTransitions(task);
        _transitionContainer.RemoveAnyTransition(task);
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
}