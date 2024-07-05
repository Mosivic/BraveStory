using GPC.Job;
using GPC.Scheduler;

internal class Landing(AbsScheduler scheduler,PlayerState state) : JobSingle(scheduler,state)
{
    protected override void _Enter()
    {
        state.Params.AnimationPlayer.Play("landing");
    }
}