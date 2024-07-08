using BraveStory.State;
using Godot;
using GPC.Job;

namespace BraveStory.Job;

internal class Test(PlayerState state) : JobBase(state)
{
    protected override void _OnStart()
    {
        GD.Print("This is Test.");
    }

    protected override void _Update(double delta)
    {
        GD.Print("This is Test Update.");
    }

    protected override bool _IsPrepared()
    {
        return true;
    }
}