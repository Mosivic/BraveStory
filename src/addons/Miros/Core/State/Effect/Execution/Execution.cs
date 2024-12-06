namespace Miros.Core;


public struct ModifierOption
{
    public string AttributeSetName { get; set; }
    public string AttributeName { get; set; }
    public float Magnitude { get; set; }
    public ModifierOperation Operation { get; set; }

    public ModifierOption(string attributeSetName, string attributeName, float magnitude, ModifierOperation operation)
    {
        AttributeSetName = attributeSetName;
        AttributeName = attributeName;
        Magnitude = magnitude;
        Operation = operation;
    }
}

public abstract class Execution
{
    public abstract void Execute(Effect effect, out ModifierOption[] modifierOptions);
}
