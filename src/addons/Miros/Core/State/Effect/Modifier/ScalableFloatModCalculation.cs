namespace Miros.Core;

public class ScalableFloatModCalculation : ModifierMagnitudeCalculation
{

    private float k = 1f;

    private float b = 0f;

    public override float CalculateMagnitude(EffectState state, float input)
    {
        return input * k + b;
    }
}