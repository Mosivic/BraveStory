using BraveStory.Player;
using FSM.Job;
using Godot;

internal class Fall(PlayerState state) : JobBase(state)
{
    protected override void _Enter()
    {
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