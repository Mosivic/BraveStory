using BraveStory.State;
using Godot;
using GPC.Job;

internal class Jump(PlayerState state) : JobBase(state)
{
    protected override void _Enter()
    {
        var velocity = state.Host.Velocity;
        state.Host.Velocity = new Vector2(velocity.X, state.Properties.JumpVelocity.Value);
        state.Host.GetNode<AnimationPlayer>("AnimationPlayer").Play("jump");
    }

    protected override void _PhysicsUpdate(double delta)
    {
        var velocity = state.Host.Velocity;
        var direction = Input.GetAxis("move_left", "move_right");
        velocity.X = direction * state.Properties.RunSpeed.Value;
        velocity.Y += state.Properties.Gravity.Value * (float)delta;
        state.Host.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            state.Host.GetNode<Sprite2D>("Sprite").FlipH = direction < 0;

        state.Host.MoveAndSlide();
        state.Host.MoveAndSlide();
    }
}