using BraveStory.State;
using Godot;
using FSM.Job;

internal class Jump(CharacterState state) : JobBase(state)
{
    protected override void _Enter()
    {
        var velocity = state.Velocity;
        state.Velocity = new Vector2(velocity.X, state.JumpVelocity.Value);
        state.AnimationPlayer.Play("jump");
    }

    protected override void _PhysicsUpdate(double delta)
    {
        var velocity = state.Velocity;
        var direction = Input.GetAxis("move_left", "move_right");
        velocity.X = direction * state.RunSpeed.Value;
        velocity.Y += state.Gravity.Value * (float)delta;
        state.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            state.Sprite.FlipH = direction < 0;

        state.MoveAndSlide();
    }
}