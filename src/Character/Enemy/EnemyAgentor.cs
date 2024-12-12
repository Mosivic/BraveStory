using System;
using Godot;
using Miros.Core;

namespace BraveStory;

public class EnemyAgentor : Agentor<Enemy,EnemyShared>
{
    public override void Binding()
    {
		var hp = Agent.GetAttributeBase("HP");
		hp.SetMaxValue(hp.CurrentValue);
		//hp.RegisterPostCurrentValueChange(Host.StatusPanel.OnUpdateHealthBar);
	}
}
