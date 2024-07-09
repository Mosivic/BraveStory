using BraveStory.State;
using GPC.Job;

internal class Idle(PlayerState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.Nodes.AnimationPlayer.Play("idle");
    }
}