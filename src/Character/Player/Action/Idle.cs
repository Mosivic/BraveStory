using Miros.Core;

namespace BraveStory;

public class IdleAction(Agent agent, Player host, PlayerContext context) : Task<State, Player,PlayerContext>(agent, host, context)
{
    public override Tag StateTag  => Tags.State_Action_Idle;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;
    public override Transition[] Transitions  => [
            new (Tags.State_Action_Run, () => Host.KeyDownMove()),
            new (Tags.State_Action_Fall, () => !Host.IsOnFloor()),
            new (Tags.State_Action_Jump, () => Host.KeyDownJump()),
            new (Tags.State_Action_Attack1, () => Host.KeyDownAttack()),
            new (Tags.State_Action_Sliding, () => Host.KeyDownSliding()),
        ];

    protected override void OnEnter()
    {
        Host.PlayAnimation("idle");
        Context.JumpCount = 0;
    }


}