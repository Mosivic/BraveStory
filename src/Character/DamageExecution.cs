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
        var targetDefense = effect.OwnerAgent.Attr("Defense");
        var targetHP = effect.OwnerAgent.Attr("HP");

        var damage = sourceAttack - targetDefense;
        var newHp = targetHP - damage;

        var hpModifier = new ModifierOption("HP", newHp, ModifierOperation.Override);

        effect.OwnerAgent.EventStream.Push("Damage", new DamageSlice(damage));

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

        effect.OwnerAgent.EventStream.Push("Damage", new DamageSlice(damage));

        modifierOptions = [hpModifier];
    }

    private readonly struct DamageData
    {
        public readonly float SourceAttack;
        public readonly float TargetDefense;
        public readonly float TargetHP;


        public DamageData(Effect effect)
        {
            SourceAttack = effect.SourceAgent.Attr("Attack");
            TargetDefense = effect.OwnerAgent.Attr("Defense");
            TargetHP = effect.OwnerAgent.Attr("HP");
        }
    }
}