namespace Miros.Core;

public enum ModifierOperation
{
    Add,
    Multiply,
    Divide,
    Override,
    Invalid
}

public enum ModifierMagnitudeType
{
    ScalableFloat
}

public struct ModifierMagnitude
{
    public ModifierMagnitudeType Type { get; set; }
}

public class Modifier
{
    public float Record;
    public required float Property { get; set; }
    public required float Affect { get; set; }
    public required ModifierOperation Operator { get; set; }
    public ModifierMagnitude Magnitude { get; set; }
}