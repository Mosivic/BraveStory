using System;
using Miros.Core;
using Godot;

namespace BraveStory;

public partial class PlayerAgentor : Agentor<Player,PlayerShared>
{

    public override void BindStators(Player host,PlayerShared shared,Type[] stators)
    {
        base.BindStators(host,shared,stators);
        
        var hp = Agent.GetAttributeBase("HP");
		hp.SetMaxValue(hp.CurrentValue);
		hp.RegisterPostCurrentValueChange(host.StatusPanel.OnUpdateHealthBar);
	}

	
}	
