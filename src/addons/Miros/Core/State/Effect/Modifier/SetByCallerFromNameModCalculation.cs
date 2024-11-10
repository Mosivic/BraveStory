using Godot;
namespace Miros.Core;

public class SetByCallerFromNameModCalculation : ModifierMagnitudeCalculation
{
    private string valueName;
    public override float CalculateMagnitude(Effect state, float input)
    {
        var value = state.GetMapValue(valueName);
#if GODOT
        if (value == null) GD.Print($"[Miros] SetByCallerModCalculation: GE's '{valueName}' value(name map) is not set");
#endif
        return value ?? 0;
    }
}
