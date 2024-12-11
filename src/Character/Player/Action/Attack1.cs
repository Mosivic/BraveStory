using Miros.Core;

namespace BraveStory;

public partial class Attack1Action : StateNode<State, Player,PlayerShared>
{
    
    public override Tag StateTag  => Tags.State_Action_Attack1;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    public override Transition[] Transitions => [
            new (Tags.State_Action_Idle),
            new (Tags.State_Action_Attack11,() => Host.KeyDownAttack(), TransitionMode.DelayFront),
        ];
    

    protected override void Enter()
    {
        Host.PlayAnimation("attack1");
    }

    protected override void PhysicsUpdate(double delta)
    {
        if(Shared.IsHit && Shared.HitAgentNode != null)
        {
            var damageEffect = new Effect()
            {
                Tag = Tags.Effect_Buff,
                Source = Agent,
                DurationPolicy = DurationPolicy.Instant,
                Executions = [new DamageExecution()]
            };

            Shared.HitAgentNode.AddState(ExecutorType.EffectExecutor, damageEffect);
            Shared.IsHit = false;
        }
    }

    protected override bool ExitCondition()
    {
        return Host.IsAnimationFinished();
    }
}
