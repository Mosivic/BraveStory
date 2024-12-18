using System;
using System.Collections.Generic;

namespace Miros.Core;

public class MultiLayerExecutor : ExecutorBase, IExecutor
{
    private readonly Dictionary<Tag, LayerExecutor> _layers = [];
    private readonly TransitionContainer _transitionContainer = new();

    public override bool HasStateRunning(State state)
    {
        return state.Status == RunningStatus.Running;
    }

    public override void SwitchStateByTag(Tag tag, Context switchArgs)
    {
        if (!_states.TryGetValue(tag, out var state))
            return;

        if (switchArgs is not MultiLayerSwitchArgs)
            Console.WriteLine("switchArgs is null or not MultiLayerSwitchArgs");

        var switchTaskArgs = switchArgs as MultiLayerSwitchArgs;
        if (_layers.TryGetValue(switchTaskArgs.Layer, out var layerExecutor))
            layerExecutor.SetNextState(state, switchTaskArgs.Mode);
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

    public override void AddState(State state)
    {
        base.AddState(state);

        var action = (ActionState)state;

        if (!_layers.ContainsKey(action.Layer))
        {
            _layers[action.Layer] = new LayerExecutor(action.Layer, _transitionContainer, _states);
            _layers[action.Layer].SetDefaultState(state); //将第一个任务设置为默认任务，以免忘记设置默认任务
        }

        if (action.Transitions != null)
            _transitionContainer.AddTransitions(state, action.Transitions);

        if (action.AsDefaultTask)
            _layers[action.Layer].SetDefaultState(state);

        if (action.AsNextTask)
            _layers[action.Layer].SetNextState(state, action.AsNextTaskTransitionMode);
    }

    public override void RemoveState(State state)
    {
        base.RemoveState(state);

        _transitionContainer.RemoveTransitions(state);
        _transitionContainer.RemoveAnyTransition(state);
    }

    public override State GetNowState(Tag layer)
    {
        return _layers.TryGetValue(layer, out var layer1) ? layer1.GetNowState() : null;
    }

    public override State GetLastState(Tag layer)
    {
        return _layers.TryGetValue(layer, out var layer1) ? layer1.GetLastState() : null;
    }
}