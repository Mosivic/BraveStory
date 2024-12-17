using Miros.Core;

namespace BraveStory;

public class Attack1ActionState : ActionState<Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag Tag => Tags.State_Action_Attack1;

    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle),
            new(Tags.State_Action_Attack11, () => Host.KeyDownAttack(), TransitionMode.DelayFront)
        ]
    );

    public override void Init(Player host, PlayerContext context, MultiLayerExecuteArgs executeArgs)
    {
        base.Init(host, context, executeArgs);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
        ExitCondition += OnExitCondition;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("attack1");
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
        return Host.IsAnimationFinished();
    }
}