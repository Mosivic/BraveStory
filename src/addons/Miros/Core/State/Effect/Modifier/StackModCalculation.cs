namespace Miros.Core;

public class StackModCalculation : ModifierMagnitudeCalculation
{
    public float b = 0;

    // 计算逻辑与ScalableFloatModCalculation一致, 公式：(StackCount) * k + b
    public float k = 1;

    public override float CalculateMagnitude(Effect state, float modifierMagnitude)
    {
        if (state.Stacking.StackingType == StackingType.None) return 0;

        var stackCount = state.Stacking.StackCount;
        return stackCount * k + b;
    }
}