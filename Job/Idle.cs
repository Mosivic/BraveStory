using BraveStory.State;
using GPC.Job;
using GPC.States;

internal class Idle(PlayerState state) : JobBase(state)
{
    protected override void _Start()
    {
        state.Nodes.AnimationPlayer.Play("idle");
    }
}