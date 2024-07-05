using Godot;
using GPC.Job;
using GPC.Scheduler;

internal class Move(PlayerState state) : JobSingle(state)
{
    protected override void _Enter()
    {
        state.Params.AnimationPlayer.Play("run");
    }

    protected override void _PhysicsUpdate(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = state.Host.Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, direction * state.RunSpeed, state.FloorAcceleration);
        velocity.Y += (float)delta * state.Gravity;
        state.Host.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            state.Params.Sprite.FlipH = direction < 0;

        state.Host.MoveAndSlide();
    }
}