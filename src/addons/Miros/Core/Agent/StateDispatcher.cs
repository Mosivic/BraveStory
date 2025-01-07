using System;
using System.Collections.Generic;

namespace Miros.Core;

public enum ExecutorType
{
    MultiLayerExecutor,
    EffectExecutor,
    CommonExecutor
}

public class StateDispatcher(Agent owner)
{
    private readonly Dictionary<ExecutorType, IExecutor> _executors = [];


    public void Update(double delta)
    {
        foreach (var executor in _executors.Values) executor.Update(delta);
    }


    public void PhysicsUpdate(double delta)
    {
        foreach (var executor in _executors.Values) executor.PhysicsUpdate(delta);
    }


    public IExecutor GetExecutor(ExecutorType executorType)
    {
        if (_executors.TryGetValue(executorType, out var executor)) return executor;

        switch (executorType)
        {
            case ExecutorType.MultiLayerExecutor:
                executor = new MultiLayerExecutor();
                break;
            case ExecutorType.EffectExecutor:
                executor = new EffectExecutor();
                break;
            case ExecutorType.CommonExecutor:
                executor = new CommonExecutor();
                break;
            default:
                throw new Exception($"Executor {executorType} not found");
        }

        _executors[executorType] = executor;
        return executor;
    }


    public void AddState(State state, Context context = null)
    {
        switch (state.StateType)
        {
            case StateType.Effect:
                AddEffect(state as Effect);
                break;
            case StateType.Action:
                AddAction(context, state.GetType());
                break;
            case StateType.State:
                state.Executor.AddState(state);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public void SwitchTaskByTag(ExecutorType executorType, Tag tag, Context switchArgs = null)
    {
        var executor = GetExecutor(executorType);
        executor.SwitchStateByTag(tag, switchArgs);
    }


    private void AddAction(Context context, Type stateType)
    {
        var state = (State)Activator.CreateInstance(stateType);

        state.Executor = GetExecutor(ExecutorType.MultiLayerExecutor) as MultiLayerExecutor;
        state.Context = context;
        state.OwnerAgent = owner;
        state.Task = TaskProvider.GetTask(state.TaskType);
        state.Init();

        state.Executor.AddState(state);
    }


    private void AddEffect(Effect effect)
    {
        effect.Executor = GetExecutor(ExecutorType.EffectExecutor) as EffectExecutor;
        effect.OwnerAgent = owner;
        effect.Task = TaskProvider.GetTask(effect.TaskType) as EffectTask;
        effect.Init();

        effect.Executor.AddState(effect);
    }
}