using Godot;
using GPC.Job;

internal class Move<T> : JobSingle<T> where T : PlayerState
{
    protected override void _Enter(T state)
    {
        state.AnimationPlayer.Play("run");
    }

    protected override void _PhysicsUpdate(T state, double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = state.Host.Velocity;
        velocity.X = direction * state.RunSpeed;
        velocity.Y += (float)delta * state.Gravity;
        state.Host.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            state.Host.GetNode<Sprite2D>("Sprite2D").FlipH = direction < 0;

        state.Host.MoveAndSlide();
    }
}