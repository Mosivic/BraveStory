using Godot;
using Miros.Core;

namespace BraveStory;

public class FallAction : Stator<State, Player,PlayerShared>
{
    public override Tag StateTag  => Tags.State_Action_Fall;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition[] Transitions  => [
			new (Tags.State_Action_Idle, () => Host.IsOnFloor()),
			new (Tags.State_Action_WallSlide, () => Host.IsHandColliding() && Host.IsFootColliding() && !Host.KeyDownMove()),
			new (Tags.State_Action_DoubleJump, () => Host.KeyDownJump() && Shared.JumpCount < Shared.MaxJumpCount),
		];
	
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

