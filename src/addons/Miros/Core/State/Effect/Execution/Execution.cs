namespace Miros.Core;

public struct ModifierOption(
    string attributeName,
    float magnitude,
    ModifierOperation operation,
    string attributeSetName = "",
    ModifierMagnitudeCalculation mmc = null)
{
    public string AttributeSetName { get; set; } = attributeSetName;
    public string AttributeName { get; set; } = attributeName;
    public float Magnitude { get; set; } = magnitude;
    public ModifierOperation Operation { get; set; } = operation;
    public ModifierMagnitudeCalculation MMC { get; set; } = mmc;
}

public abstract class Execution
{
    public abstract void Execute(Effect effect, out ModifierOption[] modifierOptions);
}