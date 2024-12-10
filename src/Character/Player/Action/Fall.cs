using Godot;
using Miros.Core;

namespace BraveStory;

public partial class FallAction : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Action_Fall;

    protected override void Enter()
    {
        Host.PlayAnimation("fall");
    }


    protected override void PhysicsUpdate(double delta)
    {
		var direction = Input.GetAxis("move_left", "move_right");
		var velocity = Host.Velocity;

		// 确保空中移动控制合理
		velocity.X = Mathf.MoveToward(
			velocity.X,
			direction * Agent.Attr("RunSpeed"),
			Agent.Attr("AirAcceleration")
		);

		velocity.Y += (float)delta * Agent.Attr("Gravity");
		Host.Velocity = velocity;

		Host.UpdateFacing(direction);
		Host.MoveAndSlide();
    }
}

