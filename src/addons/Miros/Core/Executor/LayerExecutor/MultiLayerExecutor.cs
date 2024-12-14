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

    public override void SwitchTaskByTag(Tag tag, Context context)
    {
        if (!_tasks.TryGetValue(tag, out var task))
            return;

        if (context is null || context is not MultiLayerSwitchTaskArgs)
            Console.WriteLine("context is null or not MultiLayerSwitchTaskArgs");

        var switchTaskArgs = context as MultiLayerSwitchTaskArgs;
        if (_layers.TryGetValue(switchTaskArgs.Layer, out var layerExecutor))
        {
            layerExecutor.SetNextTask(task, switchTaskArgs.Mode);
        }
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        foreach (var key in _layers.Keys) _layers[key].Update(delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var key in _layers.Keys) _layers[key].PhysicsUpdate(delta);
    }

    public override void AddTask(ITask task, Context context)
    {
        base.AddTask(task, context);

        var stateTask = task as TaskBase;
        var fsmContext = context as MultiLayerExecuteArgs;

        if (!_layers.ContainsKey(fsmContext.Layer))
        {
            _layers[fsmContext.Layer] = new LayerExecutor(fsmContext.Layer, _transitionContainer, _tasks);
            _layers[fsmContext.Layer].SetDefaultTask(stateTask); //将第一个任务设置为默认任务，以免忘记设置默认任务
        }

        if (fsmContext.Transitions != null)
            _transitionContainer.AddTransitions(stateTask, fsmContext.Transitions);

        if (fsmContext.AsDefaultTask) 
            _layers[fsmContext.Layer].SetDefaultTask(stateTask);

        if (fsmContext.AsNextTask)
            _layers[fsmContext.Layer].SetNextTask(stateTask, fsmContext.AsNextTaskTransitionMode);
    }

    public override void RemoveTask(ITask task)
    {
        base.RemoveTask(task);

        _transitionContainer.RemoveTransitions(task as TaskBase);
        _transitionContainer.RemoveAnyTransition(task as TaskBase);
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