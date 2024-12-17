using Miros.Core;

namespace BraveStory;

public class IdleActionState : ActionState<Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag Tag => Tags.State_Action_Idle;
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

    public override void Init(Player host, PlayerContext context, MultiLayerExecuteArgs executeArgs)
    {
        base.Init(host, context, executeArgs);

        EnterFunc += OnEnter;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("idle");
        Context.JumpCount = 0;
    }
}