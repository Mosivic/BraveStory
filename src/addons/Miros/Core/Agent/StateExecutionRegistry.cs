using System.Collections.Generic;
using Miros.Core;

namespace Miros.Core;

public struct StateExecutionContext(State state, TaskBase task, IExecutor executor)
{
    public State State { get; } = state;
    public TaskBase Task { get; } = task;
    public IExecutor Executor { get; } = executor;
}


public class StateExecutionRegistry
{
	private readonly Dictionary<Tag, StateExecutionContext> _STEMap = [];

	public void AddStateMap(Tag tag, StateExecutionContext stateMap)
	{
		_STEMap[tag] = stateMap;
	}

	public StateExecutionContext GetStateMap(Tag tag)
	{
		return _STEMap[tag];
	}

	public bool TryGetStateMap(Tag tag, out StateExecutionContext stateMap)
	{
		return _STEMap.TryGetValue(tag, out stateMap);
	}

	public void RemoveStateMap(Tag tag)
	{
		_STEMap.Remove(tag);
	}

	public bool HasStateMap(Tag tag)
	{
		return _STEMap.ContainsKey(tag);
	}

	public IEnumerable<StateExecutionContext> GetAllStateMaps() => _STEMap.Values;


	public void Clear()
	{
		_STEMap.Clear();
	}

	public State GetState(Tag tag) => _STEMap[tag].State;

	public State GetState(TaskBase task) => _STEMap[task.Tag].State;

	public TaskBase GetTask(Tag tag) => _STEMap[tag].Task;

	public TaskBase GetTask(State state) => _STEMap[state.Tag].Task;

	public IExecutor GetExecutor(Tag tag) => _STEMap[tag].Executor;

	public IExecutor GetExecutor(TaskBase task) => _STEMap[task.Tag].Executor;

	public IExecutor GetExecutor(State state) => _STEMap[state.Tag].Executor;
}