using Godot;

namespace Character.Enemy.Action;

public partial class StunEnemyAction : StateNode<State, Enemy>
{
    public override Tag StateTag => Tags.State_Status_Stun;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    protected override void Enter()
    {
        Host.PlayAnimation("stun");
    }
}