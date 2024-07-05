using GPC.Job;
using GPC.Scheduler;

internal class Landing(PlayerState state) : JobSingle(state)
{
    protected override void _Enter()
    {
        state.Params.AnimationPlayer.Play("landing");
    }
}