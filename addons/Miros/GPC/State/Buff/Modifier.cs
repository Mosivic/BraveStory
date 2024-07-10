namespace GPC.States.Buff;

public enum BuffModifierOperator
{
    Add,
    Multiply,
    Divide,
    Override,
    Invalid
}

public enum BuffModifierMagnitudeType
{
    ScalableFloat
}

public struct ModifierMagnitude
{
    public BuffModifierMagnitudeType Type { get; set; }
}

public class Modifier
{
    public float Record;
    public required BindableProperty<float> Property { get; set; }
    public required float Affect { get; set; }
    public required BuffModifierOperator Operator { get; set; }
    public ModifierMagnitude Magnitude { get; set; }
}