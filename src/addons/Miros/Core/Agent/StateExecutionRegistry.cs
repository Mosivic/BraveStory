using System.Collections.Generic;

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

    public void AddStateExecutionContext(Tag tag, StateExecutionContext context)
    {
        _STEMap[tag] = context;
    }

    public StateExecutionContext GetStateExecutionContext(Tag tag)
    {
        return _STEMap[tag];
    }

    public bool TryGetStateExecutionContext(Tag tag, out StateExecutionContext context)
    {
        return _STEMap.TryGetValue(tag, out context);
    }

    public void RemoveStateExecutionContext(Tag tag)
    {
        _STEMap.Remove(tag);
    }

    public bool HasStateExecutionContext(Tag tag)
    {
        return _STEMap.ContainsKey(tag);
    }

    public IEnumerable<StateExecutionContext> GetAllStateExecutionContexts()
    {
        return _STEMap.Values;
    }


    public void Clear()
    {
        _STEMap.Clear();
    }

    public State GetState(Tag tag)
    {
        return _STEMap.TryGetValue(tag, out var context) ? context.State : null;
    }

    public State GetState(TaskBase task)
    {
        return _STEMap.TryGetValue(task.Tag, out var context) ? context.State : null;
    }

    public TaskBase GetTask(Tag tag)
    {
        return _STEMap.TryGetValue(tag, out var context) ? context.Task : null;
    }

    public TaskBase GetTask(State state)
    {
        return _STEMap.TryGetValue(state.Tag, out var context) ? context.Task : null;
    }

    public IExecutor GetExecutor(Tag tag)
    {
        return _STEMap.TryGetValue(tag, out var context) ? context.Executor : null;
    }

    public IExecutor GetExecutor(TaskBase task)
    {
        return _STEMap.TryGetValue(task.Tag, out var context) ? context.Executor : null;
    }

    public IExecutor GetExecutor(State state)
    {
        return _STEMap.TryGetValue(state.Tag, out var context) ? context.Executor : null;
    }
}