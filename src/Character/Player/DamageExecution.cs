using Miros.Core;

namespace BraveStory;

public class DamageExecution : Execution
{
    public override void Execute(Effect effect, out ModifierOption[] modifierOptions)
    {
        var data = new DamageData(effect);
        var newHp = data.TargetHP - (data.SourceAttack - data.TargetDefense);
        var hpModifier = new ModifierOption("HP", newHp, ModifierOperation.Override);

        modifierOptions = [hpModifier];
    }

    private readonly struct DamageData
    {
        public readonly float SourceAttack;
        public readonly float TargetDefense;
        public readonly float TargetHP;


        public DamageData(Effect effect)
        {
            SourceAttack = effect.Source.Attr("Attack");
            TargetDefense = effect.Owner.Attr("Defense");
            TargetHP = effect.Owner.Attr("HP");
        }
    }
}