using Miros.Core;

namespace BraveStory;

public partial class Attack1Action : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Action_Attack1;

    protected override void Enter()
    {
        Host.PlayAnimation("attack1");
    }

    protected override bool ExitCondition()
    {
        return Host.IsAnimationFinished();
    }
}
