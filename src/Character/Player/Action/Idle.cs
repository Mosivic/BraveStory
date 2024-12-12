using Miros.Core;

namespace BraveStory;

public class IdleAction : Task<State, Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_Idle;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Run, () => Host.KeyDownMove()),
            new(Tags.State_Action_Fall, () => !Host.IsOnFloor()),
            new(Tags.State_Action_Jump, () => Host.KeyDownJump()),
            new(Tags.State_Action_Attack1, () => Host.KeyDownAttack()),
            new(Tags.State_Action_Sliding, () => Host.KeyDownSliding())
        ]
    );

    protected override void OnEnter()
    {
        Host.PlayAnimation("idle");
        Context.JumpCount = 0;
    }
}