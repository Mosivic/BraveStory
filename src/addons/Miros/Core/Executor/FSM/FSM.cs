using System;
using System.Collections.Generic;
using Miros.Core;

public class FSM : ExecutorBase<TaskBase>, IExecutor
{
    private readonly Dictionary<Tag, FSMExecutor> _layers = [];
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


    public override void AddTask(ITask task, StateExecuteArgs args)
    {
        if (args is not StateFSMArgs fsmArgs)
            throw new Exception("Invalid arguments for FSM ");
        
        var stateTask = task as TaskBase;
        base.AddTask(stateTask, args);
        
        if (!_layers.ContainsKey(fsmArgs.Layer))
        {
            _layers[fsmArgs.Layer] = new FSMExecutor(fsmArgs.Layer, _transitionContainer, _tasks);
            _layers[fsmArgs.Layer].SetDefaultTask(stateTask); //将第一个任务设置为默认任务
        }
        
        _transitionContainer.AddTransitions(stateTask, fsmArgs.Transitions);
        _transitionContainer.AddAnyTransition(fsmArgs.AnyTransition);
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