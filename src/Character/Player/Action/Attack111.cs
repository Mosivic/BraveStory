using Miros.Core;

namespace BraveStory;

public class Attack111ActionState : ActionState<PlayerContext>
{
    private Player _host;

    public override Tag Tag => Tags.State_Action_Attack111;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle)
    ];

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
        _host.PlayAnimation("attack111");
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
                Executions = [new CustomAttackDamageExecution(OwnerAgent.Atr("Attack") + 2)]
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