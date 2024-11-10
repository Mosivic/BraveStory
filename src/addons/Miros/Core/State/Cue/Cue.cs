
namespace Miros.Core;

public abstract class Cue: AbsState
{
    public Tag[] RequiredTags;
    public Tag[] ImmunityTags;
    protected readonly CueParameters _parameters;
    public Persona Owner { get; protected set; }

    public virtual bool Triggerable(Persona owner)
    {
        if (owner == null) return false;
        // 持有【所有】RequiredTags才可触发
        if (!owner.HasAllTags(new TagSet(RequiredTags)))
            return false;

        // 持有【任意】ImmunityTags不可触发
        if (owner.HasAnyTags(new TagSet(ImmunityTags)))
            return false;

        return true;
    }
}

