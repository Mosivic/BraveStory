using Godot;
using Miros.Core;
using Miros.EventBus;

namespace BraveStory;

public class DamageExecution : Execution
{
    public override void Execute(Effect effect, out ModifierOption[] modifierOptions)
    {
        var data = new DamageData(effect);
        var damage = data.SourceAttack - data.TargetDefense;
        var newHp = data.TargetHP - damage;
        var hpModifier = new ModifierOption("HP", newHp, ModifierOperation.Override);
      
        EventBus.Instance.Publish("Damaged", new DamagedEventArgs((int)damage, effect.Owner));

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