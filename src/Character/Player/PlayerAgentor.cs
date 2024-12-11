using System;
using Miros.Core;
using Godot;

namespace BraveStory;

public partial class PlayerAgentor : Agentor<Player, PlayerAttributeSet,PlayerShared>
{
	
	public override void Initialize(Player host, PlayerShared shared, Type[] stators)
	{
		base.Initialize(host, shared, stators);

		var hp = Agent.GetAttributeBase("HP");
		hp.SetMaxValue(hp.CurrentValue);
		hp.RegisterPostCurrentValueChange(Host.StatusPanel.OnUpdateHealthBar);
	}
}	
