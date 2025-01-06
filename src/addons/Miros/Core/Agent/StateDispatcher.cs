using System;
using System.Collections.Generic;

namespace Miros.Core;

public enum ExecutorType
{
    MultiLayerExecutor,
    EffectExecutor,
    NativeExecutor
}

public class StateDispatcher(Agent ownerAgent)
{
    private readonly Dictionary<ExecutorType, IExecutor> _executors = [];
    private readonly Agent _ownerAgent = ownerAgent;


    public void Update(double delta)
    {
        foreach (var executor in _executors.Values) executor.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var executor in _executors.Values) executor.PhysicsUpdate(delta);
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
                PushStateOnExecutor(state);
                break;
        }
    }


    public void SwitchTaskByTag(Tag tag, Context switchArgs = null)
    {
        var executor = _executors[ExecutorType.EffectExecutor];
        executor.SwitchStateByTag(tag, switchArgs);
    }


    public void AddAction(Context context, Type stateType)
    {
        var state = (State)Activator.CreateInstance(stateType);

        state.Context = context;
        state.OwnerAgent = _ownerAgent;
        state.Task = TaskProvider.GetTask(state.TaskType);
        state.Init();

        PushStateOnExecutor(state);
    }


    public void AddEffect(Effect effect)
    {
        effect.OwnerAgent = _ownerAgent;
        effect.Task = TaskProvider.GetTask(effect.TaskType) as EffectTask;
        effect.Init();

        PushStateOnExecutor(effect);
    }


    private static void PushStateOnExecutor(State state)
    {
        var executor = state.Executor;
        executor.AddState(state);
    }
}