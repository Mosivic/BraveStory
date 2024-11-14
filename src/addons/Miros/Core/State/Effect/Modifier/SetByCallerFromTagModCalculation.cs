namespace Miros.Core;

public class SetByCallerFromTagModCalculation : ModifierMagnitudeCalculation
{
    private Tag tag;

    public override float CalculateMagnitude(Effect state, float input)
    {
// 		var value = state.GetMapValue(tag);
// #if GODOT
// 		if (value == null)
// 			GD.Print($"[Miros] SetByCallerModCalculation: GE's '{tag}' value(tag map) is not set");
// #endif
// 		return value ?? 0;
        return 0;
    }
}