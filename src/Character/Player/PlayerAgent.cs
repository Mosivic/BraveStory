using Miros.Core;
using Godot;

namespace BraveStory;

public partial class PlayerAgent : Agent
{
	private StatsPanel _statusPanel;

	public override void _Ready()
	{
		base._Ready();
		Initialize(GetParent<Player>(), [typeof(PlayerAttributeSet)]);

		_statusPanel = GetNode<StatsPanel>("../CanvasLayer/StatusPanel");

		var hp = GetAttributeBase("HP");
		hp.SetMaxValue(hp.CurrentValue);
		hp.RegisterPostCurrentValueChange(_statusPanel.OnUpdateHealthBar);
		
	}

	public void HandleChildren()
	{
		var children = GetChildren();
		foreach (var child in children)
		{
			if (child is not StateNode<Player>)
				continue;

			var stateNode = child as StateNode<Player>;
			stateNode.Initialize(this, Host as Player);

			var executorType = stateNode.ExecutorType;
			if (executorType == ExecutorType.MultiLayerStateMachine)
			{
				var executor = new MultiLayerStateMachine();
				var container = new StateTransitionContainer();

				foreach (var state in states)
				{
					var task = _taskProvider.GetTask(state);
					state.Owner = this;
					_stateExecutionRegistry.AddStateExecutionContext(state.Tag,
						new StateExecutionContext(state, task, executor));
				}

				foreach (var transition in transitions.AnyTransitions)
					container.AddAny(new StateTransition(_stateExecutionRegistry.GetTask(transition.ToState),
						transition.Condition,
						transition.Mode));

				foreach (var (fromState, stateTransitions) in transitions.Transitions)
				foreach (var transition in stateTransitions)
					container.Add(_stateExecutionRegistry.GetTask(fromState),
						new StateTransition(_stateExecutionRegistry.GetTask(transition.ToState), transition.Condition,
							transition.Mode));


				executor.AddLayer(layer, _stateExecutionRegistry.GetTask(defaultState), container);
				_executors[ExecutorType.MultiLayerStateMachine] = executor;
			}
		}
	}
}
