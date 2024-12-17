using Miros.Core;

namespace BraveStory;

public class IdleEnemyAction : Action<Enemy, EnemyContext, MultiLayerExecuteArgs>
{
    public override Tag Tag => Tags.State_Action_Idle;

    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Patrol, () => !Host.IsPlayerColliding()),
            new(Tags.State_Action_Charge, () => Host.IsOnFloor() && Host.IsPlayerColliding())
        ]
    );

    public override void Init(Enemy host, EnemyContext context, MultiLayerExecuteArgs executeArgs)
    {
        base.Init(host, context, executeArgs);

        EnterFunc += OnEnter;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("idle");
    }
}