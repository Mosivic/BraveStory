using Miros.Core;
using Godot;

namespace BraveStory;

public partial class PlayerAgent : AgentNode<Player, PlayerAttributeSet>
{
	private StatsPanel _statusPanel;

	public override void _Ready()
	{
		base._Ready();
		_statusPanel = GetNode<StatsPanel>("../CanvasLayer/StatusPanel");

		var hp = Agent.GetAttributeBase("HP");
		hp.SetMaxValue(hp.CurrentValue);
		hp.RegisterPostCurrentValueChange(_statusPanel.OnUpdateHealthBar);
	}
}
