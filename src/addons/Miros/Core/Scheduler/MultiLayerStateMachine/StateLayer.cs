using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Miros.Core;

public class StateLayer
{
    private readonly TaskBase _defaultTask;
    private TaskBase _currentTask;
    private double _currentStateTime;
    private TaskBase _delayTask;
    private TaskBase _lastTask;
    private Dictionary<TaskBase, HashSet<StateTransition>> _transitionRules;
    private HashSet<StateTransition> _anyTransitionRules;

    public StateLayer(Tag layerTag, TaskBase defaultTask,
        Dictionary<TaskBase, HashSet<StateTransition>> transitionRules,
        HashSet<StateTransition> anyTransitionRules)
    {
        Layer = layerTag;
        _defaultTask = defaultTask;
        _currentTask = _defaultTask;
        _lastTask = _defaultTask;
        _delayTask = null;
        _transitionRules = transitionRules;
        _anyTransitionRules = anyTransitionRules;
    }

    public Tag Layer { get; }


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

        var transitions = GetPosibleTransitions(_currentTask);

        // 使用LINQ获取优先级最高且可进入的状态
        var nextTransition = transitions
            .OrderByDescending(t => t.ToTask.Priority)
            .Where(t => t.Mode switch
            {
                StateTransitionMode.Normal => t.CanTransition() && _currentTask.CanExit() &&
                                                t.ToTask.CanEnter(),
                StateTransitionMode.Force => t.CanTransition() && t.ToTask.CanEnter(),
                StateTransitionMode.DelayFront => t.CanTransition() && t.ToTask.CanEnter(),
                _ => throw new ArgumentException($"Unsupported transition mode: {t.Mode}")
            })
            .FirstOrDefault();

        if (nextTransition == null) return;

        if (nextTransition.Mode == StateTransitionMode.DelayFront)
        {
            _delayTask = nextTransition.ToTask;

            if (_currentTask.CanExit())
            {
                TransformState(_delayTask);
                _delayTask = null;
            }
        }
        else
        {
            TransformState(nextTransition.ToTask);
        }
    }

    private void TransformState(TaskBase nextTask)
    {
        // 检查是否可以堆叠
        // if (nextState.IsStack)
        // {
        //     nextTask.Stack(nextState.Source);
        // }

        _currentTask.Exit();
        nextTask.Enter();

        // Tags
        // _ownedTags.RemoveTag(_currentState.Tag);
        // _ownedTags.AddTag(nextState.Tag);

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

    public StateTransition[] GetPosibleTransitions(TaskBase task)
    {
        return _transitionRules[task].Union(_anyTransitionRules).ToArray();
    }
}