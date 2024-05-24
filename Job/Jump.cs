

using Godot;
using GPC.Job;
using GPC.Job.Config;

class Jump : JobSingle{
	protected override void _Enter(State state)
	{
		var mState = state as PlayerState;
		var velocity = mState.Host.Velocity;
		mState.Host.Velocity = new Vector2(velocity.X, mState.JumpVeocity);
		mState.Host.GetNode<AnimationPlayer>("AnimationPlayer").Play("idle");
	}

	protected override void _PhysicsUpdate(State state, double delta)
	{
		var mState = state as PlayerState;
		 var velocity = mState.Host.Velocity;
		 velocity.Y += mState.Gravity * (float)delta;
		 mState.Host.Velocity  = velocity;

		 mState.Host.MoveAndSlide();
	}
}
