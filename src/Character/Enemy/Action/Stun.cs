using Godot;
using Miros.Core;
namespace BraveStory;

public partial class StunEnemyAction : StateNode<State, Enemy>
{
    public override Tag StateTag => Tags.State_Action_Stun;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition[] Transitions => [
        new (Tags.State_Action_Idle, () => _stunTimer >= _stunDuration)
    ];

    protected override void Enter()
    {
        PlayAnimation("idle");
        _stunTimer = 0.0f;
        _isStunned = false;
    }

    protected override void PhysicsUpdate(double delta)
    {
        _stunTimer += (float)delta;
    }
}
