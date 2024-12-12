using Miros.Core;

namespace BraveStory;

public class Attack1Action : Task<State, Player, PlayerContext>
{
    public override Tag StateTag => Tags.State_Action_Attack1;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Idle),
        new(Tags.State_Action_Attack11, () => Host.KeyDownAttack(), TransitionMode.DelayFront)
    ];


    protected override void OnEnter()
    {
        Host.PlayAnimation("attack1");
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
                Executions = [new DamageExecution()]
            };

            Context.HitAgent.AddState(ExecutorType.EffectExecutor, damageEffect);
            Context.IsHit = false;
        }
    }

    protected override bool OnExitCondition()
    {
        return Host.IsAnimationFinished();
    }
}