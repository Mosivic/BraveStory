using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Miros.Core;

public class StateLayer
{
	public Tag Layer { get; }
	private readonly JobBase _defaultJob;
	private JobBase _currentJob;
	private JobBase _lastJob;
	private JobBase _delayJob = null;
	private readonly StateTransitionContainer _transitionContainer;
	private readonly Dictionary<Tag,JobBase> _jobs;
	private double _currentStateTime;
	
	public StateLayer(Tag layerTag,Tag defaultJobSign,
		StateTransitionContainer transitionRuleContainer,Dictionary<Tag,JobBase> jobs)
	{
		Layer = layerTag;
		_jobs = jobs;
		_defaultJob = GetJobBySign(defaultJobSign);
		_currentJob = _defaultJob;
		_lastJob = _defaultJob;
		_delayJob = null;
		_transitionContainer = transitionRuleContainer;
	}
	
	private JobBase GetJobBySign(Tag jobSign)
	{
		return _jobs.GetValueOrDefault(jobSign) ?? _defaultJob;
	}

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
		if(_delayJob != null)
		{
			if(_currentJob.CanExit())
			{
				TransformState(_delayJob);
				_delayJob = null;
				return;
			}

			return;
		}

		var transitions = _transitionContainer.GetPossibleTransition(_currentJob.Sign);

		// 使用LINQ获取优先级最高且可进入的状态
		var nextTransition = transitions
			.OrderByDescending(t => GetJobBySign(t.ToJobSign).Priority)
			.Where(t => t.Mode switch
			{
				StateTransitionMode.Normal => t.CanTransition() && _currentJob.CanExit() && GetJobBySign(t.ToJobSign).CanEnter(),
				StateTransitionMode.Force => t.CanTransition() && GetJobBySign(t.ToJobSign).CanEnter(),
				StateTransitionMode.DelayFront => t.CanTransition() && GetJobBySign(t.ToJobSign).CanEnter(),
				_ => throw new ArgumentException($"Unsupported transition mode: {t.Mode}")
			})
			.FirstOrDefault();
		
		if (nextTransition == null) return;
		
		if(nextTransition.Mode == StateTransitionMode.DelayFront)
		{
			_delayJob = GetJobBySign(nextTransition.ToJobSign);

			if(_currentJob.CanExit())
			{
				TransformState(_delayJob);
				_delayJob = null;
			}
		}
		else
		{
			TransformState(GetJobBySign(nextTransition.ToJobSign));
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
		
#if DEBUG && true
		GD.Print($"[{Engine.GetProcessFrames()}] {_lastJob.Name} -> {_currentJob.Name}.");
#endif
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
}
