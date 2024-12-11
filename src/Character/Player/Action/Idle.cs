using Miros.Core;

namespace BraveStory;

public partial class IdleAction : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Action_Idle;
    protected override Transition[] Transitions { get; init; } = [
        new (Tags.State_Action_Idle, () => Host.Velocity.Y > 0),
        new (Tags.State_Action_Idle, () => Res["Hurt"]),
    ];

    protected override void Enter()
    {
        Host.PlayAnimation("idle");
        Res["JumpCount"] = 0;
    }


}
