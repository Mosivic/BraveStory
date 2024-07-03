using GPC;
using GPC.Job;
using GPC.States;

internal class Idle(PlayerState state) : JobSingle(state)
{
    protected override void _Enter() 
    {
        state.Params.AnimationPlayer.Play("idle");
    }
}