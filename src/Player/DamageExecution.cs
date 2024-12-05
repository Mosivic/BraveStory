using Miros.Core;

namespace BraveStory;

public class DamageExecution : Execution
{
    struct DamageData
    {
        public float SourceAttack;
        public float TargetDefense;
        public float TargetHP;


        public DamageData(Effect effect)
        {
            SourceAttack = effect.Source.GetAttributeCurrentValue("Character", "Attack") ?? 0;
            TargetDefense = effect.Owner.GetAttributeCurrentValue("Character", "Defense") ?? 0;
            TargetHP = effect.Owner.GetAttributeCurrentValue("Character", "HP") ?? 0;

        }
    }

    public override void Execute(Effect effect, out Modifier[] modifiers)
    {  
        var data = new DamageData(effect);
        var newHp = data.TargetHP - (data.SourceAttack - data.TargetDefense);
        

    
        var hpModifier = new Modifier(effect.Owner.GetAttributeIdentifier("Character", "HP"), newHp, ModifierOperation.Override);

        modifiers = [hpModifier];
    }
}
