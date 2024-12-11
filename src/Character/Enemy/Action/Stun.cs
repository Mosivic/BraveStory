using Godot;
using Miros.Core;
namespace BraveStory;

public partial class StunEnemyAction : Stator<State, Enemy,EnemyShared>
{
    public override Tag StateTag => Tags.State_Action_Stun;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition[] Transitions => [
        new (Tags.State_Action_Idle, () => Shared.StunTimer >= Shared.StunDuration)
    ];

    protected override void Enter()
    {
        Host.PlayAnimation("idle");
        Shared.StunTimer = 0.0f;
        Shared.IsStunned = false;
    }

    protected override void PhysicsUpdate(double delta)
    {
        Shared.StunTimer += (float)delta;
    }
}
