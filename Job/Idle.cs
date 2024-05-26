using GPC.Job;

internal class Idle<T> : JobSingle<T> where T : PlayerState
{
    // protected override void  _absJob._Enter(T state)
    // {
    //     state.AnimationPlayer.Play("idle");
    // }
}