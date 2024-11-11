namespace Miros.Core;

public class ScalableFloatModCalculation : ModifierMagnitudeCalculation
{

    private readonly float k = 1f;

    private readonly float b = 0f;

    public override float CalculateMagnitude(Effect state, float input)
    {
        return input * k + b;
    }
}