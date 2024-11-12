using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Miros.Core;

public class StateLayer
{
	public Tag Layer { get; }
	private readonly NativeJob _defaultJob;
	private NativeJob _currentJob;
	private NativeJob _lastJob;
	private NativeJob _delayJob = null;
	private readonly StateTransitionContainer _transitionContainer;


	private double _currentStateTime;
	
	public StateLayer(Tag layerTag,NativeJob defaultJob,
		StateTransitionContainer transitionRuleContainer)
	{
		Layer = layerTag;
		_defaultJob = defaultJob;
		_currentJob = defaultJob;
		_lastJob = defaultJob;
		_delayJob = null;
		_transitionContainer = transitionRuleContainer;
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

		var transitions = _transitionContainer.GetPossibleTransition(_currentJob);

		// 使用LINQ获取优先级最高且可进入的状态
		var nextTransition = transitions
			.OrderByDescending(t => t.ToJob.Priority)
			.Where(t => t.Mode switch
			{
				StateTransitionMode.Normal => t.CanTransition() && _currentJob.CanExit() && t.ToJob.CanEnter(),
				StateTransitionMode.Force => t.CanTransition() && t.ToJob.CanEnter(),
				StateTransitionMode.DelayFront => t.CanTransition() && t.ToJob.CanEnter(),
				_ => throw new ArgumentException($"Unsupported transition mode: {t.Mode}")
			})
			.FirstOrDefault();
		
		if (nextTransition == null) return;
		
		if(nextTransition.Mode == StateTransitionMode.DelayFront)
		{
			_delayJob = nextTransition.ToJob;

			if(_currentJob.CanExit())
			{
				TransformState(nextTransition.ToJob);
				_delayJob = null;
			}
		}
		else
		{
			TransformState(nextTransition.ToJob);
		}
	}

	private void TransformState(NativeJob nextJob)
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

	public NativeJob GetNowJob()
	{
		return _currentJob;
	}

	public NativeJob GetLastJob()
	{
		return _lastJob;
	}

	public double GetCurrentStateTime()
	{
		return _currentStateTime;
	}
}
