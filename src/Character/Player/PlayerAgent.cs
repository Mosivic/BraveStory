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

}
