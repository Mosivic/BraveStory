using Miros.Core;

namespace BraveStory;

public class DamageSlice(float damage) : EventStreamArgs("Damage")
{
    public float Damage { get; } = damage;
}

public class DamageMMC : ModifierMagnitudeCalculation
{
    public override float CalculateMagnitude(Effect effect, float magnitude)
    {
        var targetDefense = effect.OwnerAgent.Atr("Defense");
        var damage = magnitude - targetDefense;

        effect.OwnerAgent.EventStream.Push("Damage", new DamageSlice(damage));

        return damage;
    }
}