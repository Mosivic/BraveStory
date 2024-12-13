using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;


// FIXME: 修复状态转换的逻辑
public class LayerExecutor
{
    private readonly Dictionary<Tag, TaskBase> _tasks = [];
    private readonly TransitionContainer _transitionContainer;

    private TaskBase _currentTask;
    private TaskBase _lastTask;
    private TaskBase _nextTask;
    private TaskBase _defaultTask;


    public LayerExecutor(Tag layerTag,
        TransitionContainer transitionContainer, Dictionary<Tag, TaskBase> tasks)
    {
        Layer = layerTag;
        _transitionContainer = transitionContainer;
        _tasks = tasks;
    }

    public Tag Layer { get; }


    public void SetDefaultTask(TaskBase task)
    {
        _defaultTask = task;
        _currentTask = _defaultTask;
        _lastTask = _defaultTask;
    }

    public void SetCurrentTask(TaskBase task, TransitionMode mode = TransitionMode.Normal)
    {

    }

    public void Update(double delta)
    {
        ProcessNextState();

        _currentTask.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        _currentTask.PhysicsUpdate(delta);
    }


    private void ProcessNextState()
    {
        if (_nextTask != null)
        {
            if (_currentTask.CanExit())
            {
                SwitchNextTask();
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
            var task = _tasks[t.To];
            if (t.Mode switch
                {
                    TransitionMode.Normal => t.CanTransition() && _currentTask.CanExit() && task.CanEnter(),
                    TransitionMode.Force => t.CanTransition() && task.CanEnter(),
                    TransitionMode.DelayFront => t.CanTransition() && task.CanEnter(),
                    TransitionMode.DelayBackend => t.CanTransition() && _currentTask.CanExit(),
                    _ => throw new ArgumentException($"Unsupported transition mode: {t.Mode}")
                })
            {
                _nextTask = task;
                if (t.Mode == TransitionMode.DelayFront)
                {
                    if (_currentTask.CanExit())
                    {
                        SwitchNextTask();
                    }
                }
                else
                {
                    SwitchNextTask();
                }
            }
        }
    }

    private void SwitchNextTask(TransitionMode mode = TransitionMode.Normal)
    {
        _currentTask.Exit();
        _nextTask.Enter();

        _lastTask = _currentTask;
        _currentTask = _nextTask;
        _nextTask = null;
    }


    public TaskBase GetNowTask()
    {
        return _currentTask;
    }

    public TaskBase GetLastTask()
    {
        return _lastTask;
    }
}