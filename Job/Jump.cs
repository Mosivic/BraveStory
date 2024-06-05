using Godot;

internal class Jump<T> : JobSingle<T> where T : PlayerState
{
    protected override void _Enter(T state)
    {
        var velocity = state.Host.Velocity;
        state.Host.Velocity = new Vector2(velocity.X, state.JumpVeocity);
        state.Host.GetNode<AnimationPlayer>("AnimationPlayer").Play("jump");
    }

    protected override void _PhysicsUpdate(T state, double delta)
    {
        var velocity = state.Host.Velocity;
        var direction = Input.GetAxis("move_left", "move_right");
        velocity.X = direction * state.RunSpeed;
        velocity.Y += state.Gravity * (float)delta;
        state.Host.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            state.Host.GetNode<Sprite2D>("Sprite2D").FlipH = direction < 0;

        state.Host.MoveAndSlide();
        state.Host.MoveAndSlide();
    }
}