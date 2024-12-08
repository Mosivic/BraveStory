namespace Miros.Core;


public struct ModifierOption(string attributeName, float magnitude, ModifierOperation operation)
{
    public string AttributeName { get; set; } = attributeName;
    public float Magnitude { get; set; } = magnitude;
    public ModifierOperation Operation { get; set; } = operation;
}

public abstract class Execution
{
    public abstract void Execute(Effect effect, out ModifierOption[] modifierOptions);
}
