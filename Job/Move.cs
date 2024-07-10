using BraveStory.State;
using Godot;
using GPC.Job;

internal class Move(CharacterState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.AnimationPlayer.Play("run");
    }

    protected override void _PhysicsUpdate(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = state.Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, direction * state.RunSpeed.Value,
            state.FloorAcceleration.Value);
        velocity.Y += (float)delta * state.Gravity.Value;
        state.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            state.Sprite.FlipH = direction < 0;

        state.MoveAndSlide();
    }
}