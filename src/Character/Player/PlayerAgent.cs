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
			if (child is not StateNode<State, Player>)
				continue;

			var stateNode = child as StateNode<State, Player>;
			stateNode.Initialize(this, Host as Player);

			var executorType = stateNode.ExecutorType;
			if (executorType == ExecutorType.MultiLayerStateMachine)
			{
				// FIXME: 状态机应该在Agent方法内部创建管理
				var executor = new FSM();
				// FIXME: 需要将状态节点转换为任务
				executor.AddTask(stateNode.StateTag, stateNode, stateNode.Transitions, stateNode.AnyTransition);
			}
		}
	}
}
