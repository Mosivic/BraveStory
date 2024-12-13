namespace Miros.Core;

public class CueTask(Cue cue) : TaskBase(cue)
{
    public override bool CanEnter()
    {
        return Triggerable(cue.OwnerAgent);
    }

    public virtual bool Triggerable(Agent owner)
    {
        if (owner == null) return false;
        // 持有【所有】RequiredTags才可触发
        if (!owner.HasAll(cue.RequiredTags))
            return false;

        // 持有【任意】ImmunityTags不可触发
        if (owner.HasAny(cue.ImmunityTags))
            return false;

        return true;
    }
}