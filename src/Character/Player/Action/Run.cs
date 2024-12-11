using Godot;
using Miros.Core;

namespace BraveStory;

public partial class RunAction : StateNode<State, Player>
{
    public override Tag StateTag  => Tags.State_Action_Run;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition[] Transitions  => [
            new (Tags.State_Action_Idle, () => !Host.KeyDownMove()),
            new (Tags.State_Action_Jump, () => Host.KeyDownJump()),
            new (Tags.State_Action_Attack1, () => Host.KeyDownAttack()),
            new (Tags.State_Action_Sliding, () => Host.KeyDownSliding()),
        ];


    protected override void Enter()
    {
        Host.PlayAnimation("run");
    }

    protected override void PhysicsUpdate(double delta)
    {
        Move(delta);
    }

    public void Move(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Host.Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, direction * Agent.Attr("RunSpeed"), Agent.Attr("FloorAcceleration"));
        velocity.Y += (float)delta * Agent.Attr("Gravity");
        Host.Velocity = velocity;

        Host.UpdateFacing(direction); // 使用新方法处理朝向
        Host.MoveAndSlide();
    }
}
