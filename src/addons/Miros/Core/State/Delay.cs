namespace Miros.Core;

public class DurationState : State
{
    public required double Duration { get; set; }

    public override void Init()
    {
        ExitCondition = OnExitCondition;
    }

    private bool OnExitCondition()
    {
        return RunningTime >= Duration;
    }
}