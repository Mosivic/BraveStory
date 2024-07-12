using BraveStory.Player;
using BraveStory.State;
using FSM.Job;
using Godot;

internal class Jump(PlayerState state) : JobBase(state)
{
    protected override void _Enter()
    {
        var velocity = state.Host.Velocity;
        state.Host.Velocity = new Vector2(velocity.X, state.Properties.JumpVelocity.Value);
        state.Host.AnimationPlayer.Play("jump");
    }

    protected override void _PhysicsUpdate(double delta)
    {
        var velocity = state.Host.Velocity;
        var direction = Input.GetAxis("move_left", "move_right");
        velocity.X = direction * state.Properties.RunSpeed.Value;
        velocity.Y += state.Properties.Gravity.Value * (float)delta;
        state.Host.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            state.Host.Sprite.FlipH = direction < 0;

        state.Host.MoveAndSlide();
    }
}