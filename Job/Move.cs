

using System.Transactions;
using Godot;
using GPC.Job;
using GPC.Job.Config;

class Move : JobSingle{
    protected override void _Enter(State state)
    {
        (state as PlayerState).AnimationPlayer.Play("run");
    }

    protected override void _PhysicsUpdate(State state, double delta)
    {
        var mState = state as PlayerState;
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = mState.Host.Velocity;
        velocity.X = direction * mState.RunSpeed;
        mState.Host.Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            mState.Host.GetNode<Sprite2D>("Sprite2D").FlipH = direction < 0;

        mState.Host.MoveAndSlide();
    }
}