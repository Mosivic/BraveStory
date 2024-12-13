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

    private TransitionMode _nextTaskTransitionMode;


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
    }

    public void SetNextTask(TaskBase task, TransitionMode mode)
    {
        _nextTask = task;
        _nextTaskTransitionMode = mode;
    }

    public void Update(double delta)
    {
        ProcessNextTask();

        _currentTask.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        _currentTask.PhysicsUpdate(delta);
    }


    private void ProcessNextTask()
    {
        if (_currentTask == null)
        {
            _currentTask = _defaultTask;
            return;
        }

        if (_nextTask != null)
        {
            SwitchNextTask();
            return;
        }


        // 当没有下一个任务时（_nextTask == null），检查当前任务是否有可转换的状态
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
                    TransitionMode.Force => t.CanTransition(),
                    TransitionMode.DelayFront => t.CanTransition() && task.CanEnter(),
                    TransitionMode.DelayBackend => t.CanTransition() && _currentTask.CanExit(),
                    _ => throw new ArgumentException($"Unsupported transition mode: {t.Mode}")
                })
            {
                _nextTask = task;
                _nextTaskTransitionMode = t.Mode;
                return;
            }
        }
    }

    private void SwitchNextTask()
    {
        if (_nextTaskTransitionMode == TransitionMode.DelayFront && !_currentTask.CanExit()) // 等待当前任务满足退出条件
            return;

        if (_nextTaskTransitionMode == TransitionMode.DelayBackend && !_nextTask.CanEnter()) // 等待新任务满足进入条件
            return;

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