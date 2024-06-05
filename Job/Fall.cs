using Godot;
using GPC.Job;
internal class Fall<T> : JobSingle,
{
    protected override void _Enter(T state)
    {
        state.AnimationPlayer.Play("fall");
    }

    protected override void _PhysicsUpdate(T state, double delta)
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