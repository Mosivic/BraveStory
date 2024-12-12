using Miros.Core;

namespace BraveStory;

public class IdleEnemyAction : Task<State, Enemy, EnemyContext>
{
    public override Tag StateTag => Tags.State_Action_Idle;

    public override Tag LayerTag => Tags.StateLayer_Movement;

    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Patrol, () => !Host.IsPlayerColliding()),
        new(Tags.State_Action_Charge, () => Host.IsPlayerColliding())
    ];

    protected override void OnEnter()
    {
        Host.PlayAnimation("idle");
    }
}