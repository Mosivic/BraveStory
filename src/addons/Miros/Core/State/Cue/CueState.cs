
namespace Miros.Core;

public abstract class CueState
{
    public GameplayTag[] RequiredTags;
    public GameplayTag[] ImmunityTags;
    protected readonly CueParameters _parameters;
    public AbilitySystemComponent Owner { get; protected set; }

    public virtual bool Triggerable(AbilitySystemComponent owner)
    {
        if (owner == null) return false;
        // 持有【所有】RequiredTags才可触发
        if (!owner.HasAllTags(new GameplayTagSet(RequiredTags)))
            return false;

        // 持有【任意】ImmunityTags不可触发
        if (owner.HasAnyTags(new GameplayTagSet(ImmunityTags)))
            return false;

        return true;
    }
}

