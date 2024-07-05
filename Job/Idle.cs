using GPC.Job;
using GPC.Scheduler;

internal class Idle(PlayerState state) : JobSingle(state)
{
    protected override void _Enter()
    {
        state.Params.AnimationPlayer.Play("idle");
    }
}