using Miros.Core;

namespace BraveStory;

public class IdleEnemyActionState : ActionState<EnemyContext>
{
    public override Tag Tag => Tags.State_Action_Idle;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Patrol, () => !_host.IsPlayerColliding()),
        new(Tags.State_Action_Charge, () => _host.IsOnFloor() && _host.IsPlayerColliding())
    ];

    private Enemy _host;
    public override void Init(EnemyContext context)
    {
        base.Init(context);
        _host = context.Host;
        EnterFunc += OnEnter;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("idle");
    }
}