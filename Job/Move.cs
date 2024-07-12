using BraveStory.Player;
using BraveStory.State;
using FSM.Job;
using Godot;

internal class Move(PlayerState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.Host.AnimationPlayer.Play("run");
    }

    protected override void _PhysicsUpdate(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = state.Host.Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, direction * state.Properties.RunSpeed.Value,
            state.Properties.FloorAcceleration.Value);
        velocity.Y += (float)delta * state.Properties.Gravity.Value;
        state.Host.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            state.Host.Sprite.FlipH = direction < 0;

        state.Host.MoveAndSlide();
    }
}