using System.Collections.Generic;

namespace Miros.Core;

public class MultiLayerStateMachineBuilder
{
    private readonly MultiLayerStateMachine _stateMachine;
    private readonly StateTaskExecutorContainer _stateTaskExecutorContainer;
    private readonly ITaskProvider _taskProvider;

    public MultiLayerStateMachineBuilder(ITaskProvider taskProvider)
    {
        _taskProvider = taskProvider;
        _stateTaskExecutorContainer = new StateTaskExecutorContainer();
        _stateMachine = new MultiLayerStateMachine();
    }

    public MultiLayerStateMachineBuilder AddStates(HashSet<State> states)
    {
        foreach (var state in states)
        {
            var task = _taskProvider.GetTask(state);
            _stateTaskExecutorContainer.AddStateMap(state.Tag, new StateTaskExecutor(state, task, _stateMachine));
        }
        return this;
    }

    public MultiLayerStateMachineBuilder ConfigureTransitions(StateTransitionConfig transitions)
    {
        var container = new StateTransitionContainer();

        // 配置Any转换
        foreach (var transition in transitions.AnyTransitions) container.AddAny(CreateStateTransition(transition));

        // 配置普通转换
        foreach (var (fromState, stateTransitions) in transitions.Transitions)
        foreach (var transition in stateTransitions)
            container.Add(
                _stateTaskExecutorContainer.GetTask(fromState),
                CreateStateTransition(transition)
            );

        return this;
    }

    private StateTransition CreateStateTransition(StateTransitionConfig.Transition transition)
    {
        return new StateTransition(
            _stateTaskExecutorContainer.GetTask(transition.ToState),
            transition.Condition,
            transition.Mode
        );
    }

    public MultiLayerStateMachine Build(Tag layer, State defaultState, StateTransitionContainer container)
    {
        _stateMachine.AddLayer(
            layer,
            _stateTaskExecutorContainer.GetTask(defaultState),
            container
        );
        return _stateMachine;
    }
}