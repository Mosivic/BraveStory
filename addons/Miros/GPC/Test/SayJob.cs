using Godot;
using GPC.Job.Config;

namespace GPC.Job;

internal class Say : JobSingle
{
    protected override void _Enter(State cfg)
    {
        GD.Print("SayJob: hello GPC!");
    }

    protected override void _Update(State cfg, double delta)
    {
        GD.Print("Running!");
    }
}