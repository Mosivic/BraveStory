namespace Miros.Core;

public class CueTask : TaskBase<Cue>
{
    public override bool CanEnter(Cue state)
    {
        return Triggerable(state.OwnerAgent, state);
    }

    public virtual bool Triggerable(Agent owner, Cue state)
    {
        if (owner == null) return false;
        // 持有【所有】RequiredTags才可触发
        if (!owner.HasAll(state.RequiredTags))
            return false;

        // 持有【任意】ImmunityTags不可触发
        if (owner.HasAny(state.ImmunityTags))
            return false;

        return true;
    }
}