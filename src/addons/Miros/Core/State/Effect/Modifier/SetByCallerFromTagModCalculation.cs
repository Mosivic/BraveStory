using Godot;
namespace Miros.Core;

public class SetByCallerFromTagModCalculation : ModifierMagnitudeCalculation
{
    private GameplayTag tag;

    public override float CalculateMagnitude(EffectState state, float input)
    {
        var value = state.GetMapValue(tag);
#if GODOT
        if (value == null)
            GD.Print($"[Miros] SetByCallerModCalculation: GE's '{tag}' value(tag map) is not set");
#endif
        return value ?? 0;
    }
}
