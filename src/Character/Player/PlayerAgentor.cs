using System;
using Miros.Core;
using Godot;

namespace BraveStory;

public partial class PlayerAgentor : Agentor<Player,PlayerShared>
{

    public override void Binding()
    {
        var hp = Agent.GetAttributeBase("HP");
		hp.SetMaxValue(hp.CurrentValue);
		//hp.RegisterPostCurrentValueChange(Host.StatusPanel.OnUpdateHealthBar);
	}

	
}	
