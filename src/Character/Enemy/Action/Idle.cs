using Miros.Core;

namespace BraveStory;

public partial class IdleEnemyAction : StateNode<State, Enemy,EnemyShared>
{
    public override Tag StateTag => Tags.State_Action_Idle;

    public override Tag LayerTag => Tags.StateLayer_Movement;

    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    public override Transition[] Transitions => [
        new (Tags.State_Action_Walk, () => !Host.IsPlayerColliding()),
        new (Tags.State_Action_Run, () => Host.IsPlayerColliding())
    ];

    protected override void Enter()
    {
        Host.PlayAnimation("idle");
    }
}

