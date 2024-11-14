namespace Miros.Core;

public class CueJob(Cue cue) : JobBase(cue)
{
    public override bool CanEnter()
    {
        return Triggerable(cue.Owner);
    }

    public virtual bool Triggerable(Persona owner)
    {
        if (owner == null) return false;
        // 持有【所有】RequiredTags才可触发
        if (!owner.HasAllTags(cue.RequiredTags))
            return false;

        // 持有【任意】ImmunityTags不可触发
        if (owner.HasAnyTags(cue.ImmunityTags))
            return false;

        return true;
    }
}