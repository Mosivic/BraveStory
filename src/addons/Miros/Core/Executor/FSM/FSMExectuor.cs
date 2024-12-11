using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class FSMExecutor
{
    public Tag Layer { get; }
    private TaskBase _defaultTask;
    private readonly TransitionContainer _transitionContainer;
    private readonly Dictionary<Tag, TaskBase> _tasks = [];
    private double _currentStateTime;
    private TaskBase _currentTask;
    private TaskBase _delayTask;
    private TaskBase _lastTask;

    public FSMExecutor(Tag layerTag,
        TransitionContainer transitionContainer, Dictionary<Tag, TaskBase> tasks)
    {
        Layer = layerTag;
        _currentTask = _defaultTask;
        _lastTask = _defaultTask;
        _delayTask = null;
        _transitionContainer = transitionContainer;
        _tasks = tasks;
    }

    public void SetDefaultTask(TaskBase task)
    {
        _defaultTask = task;
    }

    public void Update(double delta)
    {
        ProcessNextState();

        _currentTask.Update(delta);
        _currentStateTime += delta;
    }

    public void PhysicsUpdate(double delta)
    {
        _currentTask.PhysicsUpdate(delta);
    }


    private void ProcessNextState()
    {
        if (_delayTask != null)
        {
            if (_currentTask.CanExit())
            {
                TransformState(_delayTask);
                _delayTask = null;
                return;
            }

            return;
        }

        var transitions = _transitionContainer.GetPossibleTransition(_currentTask);

        // 使用LINQ获取优先级最高且可进入的状态
        var sortedTransitions = transitions
            .OrderByDescending(t => t.Priority)
            .ToArray();

        if (sortedTransitions.Length == 0) return; // 没有可转换的状态
        
        foreach (var t in sortedTransitions)
        {
            TaskBase task = _tasks[t.To];
            if (t.Mode switch
            {
                TransitionMode.Normal => t.CanTransition() && _currentTask.CanExit() && task.CanEnter(),
                TransitionMode.Force => t.CanTransition() && task.CanEnter(),
                TransitionMode.DelayFront => t.CanTransition() && task.CanEnter(),
                TransitionMode.DelayBackend => t.CanTransition() && _currentTask.CanExit(),
                _ => throw new ArgumentException($"Unsupported transition mode: {t.Mode}")
            })
            {

                if (t.Mode == TransitionMode.DelayFront)
                {
                    _delayTask = task;

                    if (_currentTask.CanExit())
                    {
                        TransformState(_delayTask);
                        _delayTask = null;
                    }
                }
                else
                {
                    TransformState(task);
                }
            }
        }
    }

    private void TransformState(TaskBase nextTask)
    {
        _currentTask.Exit();
        nextTask.Enter();

        _lastTask = _currentTask;
        _currentTask = nextTask;
        _currentStateTime = 0.0;
    }


    public TaskBase GetNowTask()
    {
        return _currentTask;
    }

    public TaskBase GetLastTask()
    {
        return _lastTask;
    }

    public double GetCurrentTaskTime()
    {
        return _currentStateTime;
    }
}