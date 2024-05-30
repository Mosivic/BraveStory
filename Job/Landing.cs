using GPC.Job;

internal class Landing<T> : JobSingle<T> where T : PlayerState
{
    protected override void _Enter(T state)
    {
        state.AnimationPlayer.Play("landing");
    }
}