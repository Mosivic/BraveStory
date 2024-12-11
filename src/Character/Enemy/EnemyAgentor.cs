using System;
using Godot;
using Miros.Core;

namespace BraveStory;

public class EnemyAgentor : Agentor<Enemy,BoarAttributeSet,EnemyShared>
{
	public override void Initialize(Enemy host, EnemyShared shared, Type[] stators)
	{
		base.Initialize(host, shared, stators);

		var hp = Agent.GetAttributeBase("HP");
		hp.SetMaxValue(hp.CurrentValue);
		hp.RegisterPostCurrentValueChange(Host.StatusPanel.OnUpdateHealthBar);
	}
}
