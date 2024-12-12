using Miros.Core;

namespace BraveStory;

public class Attack11Action : Task<State, Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_Attack11;

    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle),
            new(Tags.State_Action_Attack111, () => Host.KeyDownAttack(), TransitionMode.DelayFront)
        ]
    );


    protected override void OnEnter()
    {
        Host.PlayAnimation("attack11");
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        if (Context.IsHit && Context.HitAgent != null)
        {
            var damageEffect = new Effect
            {
                Tag = Tags.Effect_Buff,
                Source = Agent,
                DurationPolicy = DurationPolicy.Instant,
                Executions = [new CustomAttackDamageExecution(Agent.Attr("Attack") + 1)]
            };

            Context.HitAgent.AddTaskFromState(ExecutorType.EffectExecutor, damageEffect);
            Context.IsHit = false;
        }
    }

    protected override bool OnExitCondition()
    {
        return Host.IsAnimationFinished();
    }
}