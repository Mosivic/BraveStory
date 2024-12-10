using Miros.Core;

namespace BraveStory;

public partial class IdleAction : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Action_Idle;

    protected override void Enter()
    {
        Host.PlayAnimation("idle");
        Res["JumpCount"] = 0;
    }


}
