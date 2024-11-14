using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Miros.Core;

public class StateLayer
{
    private readonly JobBase _defaultJob;
    private JobBase _currentJob;
    private double _currentStateTime;
    private JobBase _delayJob;
    private JobBase _lastJob;
    private Dictionary<JobBase, HashSet<StateTransition>> _transitionRules;
    private HashSet<StateTransition> _anyTransitionRules;

    public StateLayer(Tag layerTag, JobBase defaultJob,
        Dictionary<JobBase, HashSet<StateTransition>> transitionRules,
        HashSet<StateTransition> anyTransitionRules)
    {
        Layer = layerTag;
        _defaultJob = defaultJob;
        _currentJob = _defaultJob;
        _lastJob = _defaultJob;
        _delayJob = null;
        _transitionRules = transitionRules;
        _anyTransitionRules = anyTransitionRules;
    }

    public Tag Layer { get; }


    public void Update(double delta)
    {
        ProcessNextState();

        _currentJob.Update(delta);
        _currentStateTime += delta;
    }

    public void PhysicsUpdate(double delta)
    {
        _currentJob.PhysicsUpdate(delta);
    }


    private void ProcessNextState()
    {
        if (_delayJob != null)
        {
            if (_currentJob.CanExit())
            {
                TransformState(_delayJob);
                _delayJob = null;
                return;
            }

            return;
        }

        var transitions = GetPosibleTransitions(_currentJob);

        // 使用LINQ获取优先级最高且可进入的状态
        var nextTransition = transitions
            .OrderByDescending(t => t.ToJob.Priority)
            .Where(t => t.Mode switch
            {
                StateTransitionMode.Normal => t.CanTransition() && _currentJob.CanExit() &&
                                                t.ToJob.CanEnter(),
                StateTransitionMode.Force => t.CanTransition() && t.ToJob.CanEnter(),
                StateTransitionMode.DelayFront => t.CanTransition() && t.ToJob.CanEnter(),
                _ => throw new ArgumentException($"Unsupported transition mode: {t.Mode}")
            })
            .FirstOrDefault();

        if (nextTransition == null) return;

        if (nextTransition.Mode == StateTransitionMode.DelayFront)
        {
            _delayJob = nextTransition.ToJob;

            if (_currentJob.CanExit())
            {
                TransformState(_delayJob);
                _delayJob = null;
            }
        }
        else
        {
            TransformState(nextTransition.ToJob);
        }
    }

    private void TransformState(JobBase nextJob)
    {
        // 检查是否可以堆叠
        // if (nextState.IsStack)
        // {
        //     nextJob.Stack(nextState.Source);
        // }

        _currentJob.Exit();
        nextJob.Enter();

        // Tags
        // _ownedTags.RemoveTag(_currentState.Tag);
        // _ownedTags.AddTag(nextState.Tag);

        _lastJob = _currentJob;
        _currentJob = nextJob;
        _currentStateTime = 0.0;
        
    }

    public JobBase GetNowJob()
    {
        return _currentJob;
    }

    public JobBase GetLastJob()
    {
        return _lastJob;
    }

    public double GetCurrentJobTime()
    {
        return _currentStateTime;
    }

    public StateTransition[] GetPosibleTransitions(JobBase job)
    {
        return _transitionRules[job].Union(_anyTransitionRules).ToArray();
    }
}