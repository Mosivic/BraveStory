

namespace Miros.Core;

public abstract class CueDurational : Cue
{

    public override Cue ApplyFrom(Effect effect)
    {
        if (!Triggerable(effect.Owner)) return null;
        _parameters = new CueParameters { sourceEffect = effect };
        return this;
    }

    public override Cue ApplyFrom(Ability ability, params object[] customArguments)
    {
        if (!Triggerable(ability.Owner)) return null;
        _parameters = new CueParameters { sourceAbility = ability, customArguments = customArguments };
        return this;
    }
}



