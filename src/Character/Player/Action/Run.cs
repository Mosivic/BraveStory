using Godot;
using Miros.Core;

namespace BraveStory;

public partial class RunAction : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Action_Run;

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
