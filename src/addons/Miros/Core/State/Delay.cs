namespace Miros.Core;

public class DelayState : State
{
    public virtual double DelayTime { get; set; }

    public override void Init()
    {
        ExitCondition = OnExitCondition;
    }

    private bool OnExitCondition()
    {
        return RunningTime >= DelayTime;
    }
}