namespace Miros.Core;

public class SetByCallerFromNameModCalculation : ModifierMagnitudeCalculation
{
    private string valueName;

    public override float CalculateMagnitude(Effect effect, float magnitude)
    {
//         var value = state.GetMapValue(valueName);
// #if GODOT
//         if (value == null) GD.Print($"[Miros] SetByCallerModCalculation: GE's '{valueName}' value(name map) is not set");
// #endif
//         return value ?? 0;
        return 0;
    }
}