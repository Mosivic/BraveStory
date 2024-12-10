using Miros.Core;

namespace BraveStory;

public partial class HitAction : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Action_Hit;


    protected override void ShareRes()
    {
        Res["Hurt"] = true;
    }
    protected override void Enter()
    {
        Host.PlayAnimation("hit");
        Res["Hurt"] = false;
    }


}
