using Miros.Core;

namespace BraveStory;

public class Attack1ActionState : ActionState<PlayerContext>
{
    public override Tag Tag => Tags.State_Action_Attack1;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle),
        new(Tags.State_Action_Attack11, () => _host.KeyDownAttack(), TransitionMode.DelayFront)
    ];

    private Player _host;

    public override void Init(PlayerContext context)
    {
        base.Init(context);
        _host = context.Host;

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
        ExitCondition += OnExitCondition;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("attack1");
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (Context.IsHit && Context.HitAgent != null)
        {
            var damageEffect = new Effect
            {
                Tag = Tags.Effect_Buff,
                SourceAgent = OwnerAgent,
                RemovePolicy = RemovePolicy.WhenExited,
                DurationPolicy = DurationPolicy.Instant,
                Executions = [new CustomAttackDamageExecution(OwnerAgent.Atr("Attack") - 1)]
            };

            Context.HitAgent.AddEffect(damageEffect);
            Context.IsHit = false;
        }
    }

    private bool OnExitCondition()
    {
        return _host.IsAnimationFinished();
    }
}