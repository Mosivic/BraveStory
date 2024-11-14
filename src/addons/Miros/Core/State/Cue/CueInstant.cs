namespace Miros.Core;

public abstract class CueInstant(string name, Tag sign) : Cue(name, sign)
{
    // public override Cue ApplyFrom(Effect effect)
    // {
    //     if (Triggerable(effect.Owner))
    //     {
    //         SourceEffect = effect ;
    //         Trigger();
    //     }
    //     return this;
    // }

    // public override Cue ApplyFrom(Ability ability, params object[] customArguments)
    // {
    //     if (Triggerable(ability.Owner))
    //     {
    //         SourceAbility = ability;
    //         CustomArguments = customArguments;
    //         Trigger();
    //     }
    //     return this;
    // }
}