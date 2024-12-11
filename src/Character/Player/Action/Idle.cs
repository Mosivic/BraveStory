using Miros.Core;

namespace BraveStory;

public partial class IdleAction : StateNode<Player>
{
    protected override Tag StateTag => Tags.State_Action_Idle;
    protected override Tag LayerTag => Tags.StateLayer_Movement;
    protected override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    protected override Transition[] Transitions  => [
            new (Tags.State_Action_Run, () => Host.KeyDownMove()),
            new (Tags.State_Action_Fall, () => !Host.IsOnFloor()),
            new (Tags.State_Action_Jump, () => Host.KeyDownJump()),
            new (Tags.State_Action_Attack1, () => Host.KeyDownAttack()),
            new (Tags.State_Action_Sliding, () => Host.KeyDownSliding()),
        ];
    


    protected override void Enter()
    {
        Host.PlayAnimation("idle");
        Res["JumpCount"] = 0;
    }


}
