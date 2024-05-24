

using Godot;
using GPC.Job;
using GPC.Job.Config;

class Idle : JobSingle{
    protected override void _Enter(State state)
    {
        var mState = state as PlayerState; 
        mState.AnimationPlayer.Play("idle");
    }
}