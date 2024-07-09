using BraveStory.State;
using GPC.Job;

internal class Landing(PlayerState state) : JobBase(state)
{
    protected override void _Start()
    {
        state.Nodes.AnimationPlayer.Play("landing");
    }
}