using BraveStory.State;
using GPC.Job;

internal class Idle(CharacterState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.AnimationPlayer.Play("idle");
    }
}