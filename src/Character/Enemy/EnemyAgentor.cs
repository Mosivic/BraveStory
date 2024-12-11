using Godot;
using Miros.Core;

namespace BraveStory;

public class EnemyAgent : Agentor<Enemy,BoarAttributeSet,EnemyShared>
{
	private StatsPanel _statusPanel;

	public override void _Ready()
	{
		base._Ready();

		_statusPanel = GetNode<StatsPanel>("../StatusPanel");

		var hp = Agent.GetAttributeBase("HP");
		hp.SetMaxValue(hp.CurrentValue);
		hp.RegisterPostCurrentValueChange(_statusPanel.OnUpdateHealthBar);
	}
}
