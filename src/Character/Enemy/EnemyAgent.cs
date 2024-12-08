using Godot;
using Miros.Core;

namespace BraveStory;

public partial class EnemyAgent : Agent
{
	private StatsPanel _statusPanel;

	public override void _Ready()
	{
		base._Ready();

		_statusPanel = GetNode<StatsPanel>("../StatusPanel");

		Initialize(GetParent<Node2D>(), [typeof(BoarAttributeSet)]);

		var hp = GetAttributeBase("HP");
		hp.SetMaxValue(hp.CurrentValue);
		hp.RegisterPostCurrentValueChange(_statusPanel.OnUpdateHealthBar);
	}
}
