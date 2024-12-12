using Miros.Core;

namespace BraveStory;

public class HurtAction : Task<State, Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_Hurt;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle, () => Host.IsAnimationFinished()),
            new(Tags.State_Action_Hurt, () => Context.IsHurt, TransitionMode.Force, 0, true)
        ]
    );

    protected override void OnEnter()
    {
        Host.PlayAnimation("hurt");
        Context.IsHurt = false;
    }
}