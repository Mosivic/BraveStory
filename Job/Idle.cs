using BraveStory.State;
using GPC.Job;

internal class Idle(PlayerState state) : JobBase(state)
{
    protected override void _OnStart()
    {
        state.Nodes.AnimationPlayer.Play("idle");
    }
}