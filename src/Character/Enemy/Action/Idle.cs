using Miros.Core;

namespace BraveStory;

public class IdleEnemyActionState : ActionState<Enemy, EnemyContext>
{
    public override Tag Tag => Tags.State_Action_Idle;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Patrol, () => !Host.IsPlayerColliding()),
        new(Tags.State_Action_Charge, () => Host.IsOnFloor() && Host.IsPlayerColliding())
    ];

    public override void Init(Enemy host, EnemyContext context)
    {
        base.Init(host, context);

        EnterFunc += OnEnter;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("idle");
    }
}