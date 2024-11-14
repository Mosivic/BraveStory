namespace Miros.Core;

public class ScalableFloatModCalculation : ModifierMagnitudeCalculation
{
    private readonly float b = 0f;

    private readonly float k = 1f;

    public override float CalculateMagnitude(Effect state, float input)
    {
        return input * k + b;
    }
}