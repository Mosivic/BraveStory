
namespace Miros.Core;

public abstract class CueInstant : Cue  
{
    public override Cue ApplyFrom(Effect effect)
    {
        if (Triggerable(effect.Owner))
        {
            _parameters = new CueParameters { sourceEffect = effect };
            Trigger();
        }
        return this;
    }

    public override Cue ApplyFrom(Ability ability, params object[] customArguments)
    {
        if (Triggerable(ability.Owner))
        {
            _parameters = new CueParameters { sourceAbility = ability, customArguments = customArguments };
            Trigger();
        }
        return this;
    }

}