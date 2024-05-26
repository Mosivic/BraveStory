using Godot;
using GPC.Job;

internal class Jump<T> : JobSingle<T> where T : PlayerState
{
    protected override void _Enter(T state)
    {
        var velocity = state.Host.Velocity;
        state.Host.Velocity = new Vector2(velocity.X, state.JumpVeocity);
        state.Host.GetNode<AnimationPlayer>("AnimationPlayer").Play("idle");
    }

    protected override void _PhysicsUpdate(T state, double delta)
    {
        var velocity = state.Host.Velocity;
        velocity.Y += state.Gravity * (float)delta;
        state.Host.Velocity = velocity;

        state.Host.MoveAndSlide();
    }
}