using Miros.Core;

namespace BraveStory;

public class StunEnemyAction : Task<State, Enemy, EnemyContext>
{
    public override Tag StateTag => Tags.State_Action_Stun;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Idle, () => Context.StunTimer >= Context.StunDuration)
    ];

    protected override void OnEnter()
    {
        Host.PlayAnimation("idle");
        Context.StunTimer = 0.0f;
        Context.IsStunned = false;
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        Context.StunTimer += (float)delta;
    }
}