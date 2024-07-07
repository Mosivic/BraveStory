using BraveStory.State;
using GPC.Job;

internal class Landing(PlayerState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.Nodes.AnimationPlayer.Play("landing");
    }
}