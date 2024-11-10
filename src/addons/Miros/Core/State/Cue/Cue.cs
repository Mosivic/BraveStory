
namespace Miros.Core;

public abstract class Cue: AbsState
{
    public Tag[] RequiredTags;
    public Tag[] ImmunityTags;
    protected CueParameters _parameters { get; set; }
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


    public abstract Cue ApplyFrom(Effect effect);
    public abstract Cue ApplyFrom(Ability ability, params object[] customArguments);
    public abstract void Trigger();

    public abstract void OnAdd();
    public abstract void OnRemove();
    public abstract void OnGameplayEffectActivate();
    public abstract void OnGameplayEffectDeactivate();
    public abstract void OnTick();
}

