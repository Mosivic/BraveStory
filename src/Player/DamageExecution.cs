using Miros.Core;

namespace BraveStory;

public class DamageExecution : Execution
{
    public override void Execute(Effect effect, out Modifier[] modifiers)
    {
        var data = new DamageData(effect);
        var newHp = data.TargetHP - (data.SourceAttack - data.TargetDefense);


        var hpModifier = new Modifier(effect.Owner.GetAttributeIdentifier("Character", "HP"), newHp,
            ModifierOperation.Override);

        modifiers = [hpModifier];
    }

    private struct DamageData
    {
        public readonly float SourceAttack;
        public readonly float TargetDefense;
        public readonly float TargetHP;


        public DamageData(Effect effect)
        {
            SourceAttack = effect.Source.GetAttributeCurrentValue("Character", "Attack") ?? 0;
            TargetDefense = effect.Owner.GetAttributeCurrentValue("Character", "Defense") ?? 0;
            TargetHP = effect.Owner.GetAttributeCurrentValue("Character", "HP") ?? 0;
        }
    }
}