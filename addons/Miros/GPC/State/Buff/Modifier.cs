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
    public float Recored;
    public BindableProperty<float> Property { get; set; }
    public float Affect { get; set; }
    public BuffModifierOperator Operator { get; set; }
    public ModifierMagnitude Magnitude { get; set; }
}