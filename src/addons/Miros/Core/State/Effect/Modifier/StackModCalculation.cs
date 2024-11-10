

namespace Miros.Core;

public class StackModCalculation : ModifierMagnitudeCalculation
{
    // 计算逻辑与ScalableFloatModCalculation一致, 公式：(StackCount) * k + b
    public float k = 1;

    public float b = 0;

    public override float CalculateMagnitude(Effect state, float modifierMagnitude)
    {
        if (state.Stacking.stackingType == StackingType.None) return 0;

        var stackCount = state.StackCount;
        return stackCount * k + b;
    }
}
