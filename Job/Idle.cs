

using Godot;
using GPC.Job;
using GPC.Job.Config;

class Idle<T> : JobSingle<T> where T : PlayerState
{
    // protected override void  _absJob._Enter(T state)
    // {
    //     state.AnimationPlayer.Play("idle");
    // }
}


