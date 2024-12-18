using Miros.Core;

namespace BraveStory;

public class IdleEnemyActionState : ActionState
{
    private EnemyContext ctx;

    private Enemy host;
    public override Tag Tag => Tags.State_Action_Idle;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override bool AsDefaultTask => true;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Patrol, () => !host.IsPlayerColliding()),
        new(Tags.State_Action_Charge, () => host.IsOnFloor() && host.IsPlayerColliding())
    ];

    public override void Init()
    {
        ctx = Context as EnemyContext;
        host = ctx.Host;

        EnterFunc = OnEnter;
    }

    private void OnEnter()
    {
        host.PlayAnimation("idle");
    }
}