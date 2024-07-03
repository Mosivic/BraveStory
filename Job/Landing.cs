using GPC.Job;

internal class Landing(PlayerState state) : JobSingle(state)
{
    protected override void _Enter()
    {
        state.Params.AnimationPlayer.Play("landing");
    }
}