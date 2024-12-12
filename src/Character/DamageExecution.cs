using Miros.Core;

namespace BraveStory;

public class DamageSlice(float damage) : EventStreamArgs("Damage")
{
    public float Damage { get; } = damage;
}


public class CustomAttackDamageExecution(float sourceAttack) : Execution
{
    public override void Execute(Effect effect, out ModifierOption[] modifierOptions)
    {
        var targetDefense = effect.Owner.Attr("Defense");
        var targetHP = effect.Owner.Attr("HP");

        var damage = sourceAttack - targetDefense;
        var newHp = targetHP - damage;

        var hpModifier = new ModifierOption("HP", newHp, ModifierOperation.Override);

        effect.Owner.EventStream.Push("Damage", new DamageSlice(damage));

        modifierOptions = [hpModifier];
    }
}

public class DamageExecution : Execution
{
    public override void Execute(Effect effect, out ModifierOption[] modifierOptions)
    {
        var data = new DamageData(effect);
        var damage = data.SourceAttack - data.TargetDefense;
        var newHp = data.TargetHP - damage;
        var hpModifier = new ModifierOption("HP", newHp, ModifierOperation.Override);

        effect.Owner.EventStream.Push("Damage", new DamageSlice(damage));

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