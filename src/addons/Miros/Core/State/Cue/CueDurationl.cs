namespace Miros.Core;

public abstract class CueDurational(string name, Tag sign) : Cue(name, sign)
{
    // public override Cue ApplyFrom(Effect effect)
    // {
    //     if (!Triggerable(effect.Owner)) return null;
    //     SourceEffect = effect;
    //     return this;
    // }

    // public override Cue ApplyFrom(Ability ability, params object[] customArguments)
    // {
    //     if (!Triggerable(ability.Owner)) return null;
    //     SourceAbility = ability;
    //     CustomArguments = customArguments;
    //     return this;
    // }
}