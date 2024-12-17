using System;
using System.Collections.Generic;

namespace Miros.Core;

public class MultiLayerExecutor : ExecutorBase<State>, IExecutor<State>
{
    private readonly Dictionary<Tag, LayerExecutor> _layers = [];
    private readonly TransitionContainer _transitionContainer = new();

    public override bool HasStateRunning(State state)
    {
        return state.Status == RunningStatus.Running;
    }

    public override void SwitchStateByTag(Tag tag, Context context)
    {
        if (!_states.TryGetValue(tag, out var state))
            return;

        if (context is null || context is not MultiLayerSwitchTaskArgs)
            Console.WriteLine("context is null or not MultiLayerSwitchTaskArgs");

        var switchTaskArgs = context as MultiLayerSwitchTaskArgs;
        if (_layers.TryGetValue(switchTaskArgs.Layer, out var layerExecutor))
        {
            layerExecutor.SetNextState(state, switchTaskArgs.Mode);
        }
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        foreach (var key in _layers.Keys) _layers[key].Update(delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var key in _layers.Keys) _layers[key].PhysicsUpdate(delta);
    }

    public override void AddState(State state, Context context)
    {
        base.AddState(state, context);

        var fsmContext = context as MultiLayerExecuteArgs;

        if (!_layers.ContainsKey(fsmContext.Layer))
        {
            _layers[fsmContext.Layer] = new LayerExecutor(fsmContext.Layer, _transitionContainer, _states);
            _layers[fsmContext.Layer].SetDefaultState(state); //将第一个任务设置为默认任务，以免忘记设置默认任务
        }

        if (fsmContext.Transitions != null)
            _transitionContainer.AddTransitions(state, fsmContext.Transitions);

        if (fsmContext.AsDefaultTask) 
            _layers[fsmContext.Layer].SetDefaultState(state);

        if (fsmContext.AsNextTask)
            _layers[fsmContext.Layer].SetNextState(state, fsmContext.AsNextTaskTransitionMode);
    }

    public override void RemoveState(State state)
    {
        base.RemoveState(state);

        _transitionContainer.RemoveTransitions(state);
        _transitionContainer.RemoveAnyTransition(state);
    }

    public override State GetNowState(Tag layer)
    {
        if (_layers.ContainsKey(layer)) return _layers[layer].GetNowState();
        return null;
    }

    public override State GetLastState(Tag layer)
    {
        if (_layers.ContainsKey(layer)) return _layers[layer].GetLastState();
        return null;
    }
}