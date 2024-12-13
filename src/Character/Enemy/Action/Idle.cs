using Miros.Core;

namespace BraveStory;

public class IdleEnemyAction : Task<State, Enemy, EnemyContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_Idle;

    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Patrol, () => !Host.IsPlayerColliding()),
            new(Tags.State_Action_Charge, () => Host.IsPlayerColliding())
        ]
    );

    protected override void OnEnter()
    {
        Host.PlayAnimation("idle");
    }
}