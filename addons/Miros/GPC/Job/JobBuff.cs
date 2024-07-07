using GPC.States;
using GPC.States.Buff;

namespace GPC.Job;

public class JobBuff(Buff buff) : JobBase(buff)
{
    public override void Enter()
    {
        State.IsRunning = true;
        State.RunningResult = JobRunningResult.NoResult;
        
        switch (buff.DurationPolicy)
        {
            case BuffDurationPolicy.Instand:
                for (int i = 0; i < buff.Modifiers.Count; i++)
                {
                    var modifier = buff.Modifiers[i];
                    switch (modifier.Operator)
                    {
                        case BuffModifierOperator.Add:
                                modifier.Property+= modifier.Affect;
                            break;
                        case BuffModifierOperator.Multiply:
                                modifier.Property *= modifier.Affect;
                            break;
                        case BuffModifierOperator.Divide:
                                modifier.Property /= modifier.Affect;
                            break;
                        case BuffModifierOperator.Override:
                                modifier.Property = modifier.Affect; ;
                            break;
                    }
                }

                State.IsRunning = false;
                State.RunningResult = JobRunningResult.Succeed;
                break;
            case BuffDurationPolicy.Infinite:
                break;
            case BuffDurationPolicy.Interval:
                break;
        }
    }


}