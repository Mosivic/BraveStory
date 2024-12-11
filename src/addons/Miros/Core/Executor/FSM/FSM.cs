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


    public void AddTask(Tag layer, TaskBase task, Transition[] transitions, Transition anyTransition)
    {
        if (!_layers.ContainsKey(layer))
        {
            _layers[layer] = new FSMExecutor(layer, _transitionContainer, _tasks);
        }
        _tasks.Add(task.Tag, task);
        _transitionContainer.AddTransitions(task, transitions);
        _transitionContainer.AddAnyTransition(anyTransition);
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