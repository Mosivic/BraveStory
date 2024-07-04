using Godot;
using GPC.Job;

internal class Fall(PlayerState state) : JobSingle(state)
{
    protected override void _Enter()
    {
        state.Params.AnimationPlayer.Play("fall");
    }

    protected override void _PhysicsUpdate(double delta)
    {
        var velocity = state.Host.Velocity;
        var direction = Input.GetAxis("move_left", "move_right");
        velocity.X = Mathf.MoveToward(velocity.X, direction * state.RunSpeed, state.AirAcceleration);
        velocity.Y += state.Gravity * (float)delta;
        state.Host.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            state.Host.GetNode<Sprite2D>("Sprite2D").FlipH = direction < 0;

        state.Host.MoveAndSlide();
    }
}