using Godot;
using GPC.Job;
using GPC.Scheduler;

namespace BraveStory.Job;

internal class Test(PlayerState state) : JobSingle(state)
{
    protected override void _Enter()
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