namespace Miros.Core;


public interface ICalculator
{
    float Calculate(Effect effect);
}

public enum ModifierMagnitudeType
{
    ScalableFloat,
    AttributeBased,
    AttributeBasedWithStack,
    SetByCallerFromName,
    SetByCallerFromTag
}

public struct ModifierMagnitude
{
    public ModifierMagnitudeType Type { get; set; }
}

public class Modifier:ICalculator
{
    public ModifierMagnitudeCalculation MMC; // 幅度计算


    public Modifier(Tag attributeTag, float magnitude, ModifierOperation operation,
        ModifierMagnitudeCalculation mmc)
    {
        AttributeTag = attributeTag;
        Magnitude = magnitude;
        Operation = operation;
        MMC = mmc;
    }

    public Modifier(Tag attributeSetTag, Tag attributeTag, float magnitude, ModifierOperation operation,
        ModifierMagnitudeCalculation mmc)
    {
        AttributeSetTag = attributeSetTag;
        AttributeTag = attributeTag;
        Magnitude = magnitude;
        Operation = operation;
        MMC = mmc;
    }

    public Tag AttributeSetTag { get; set; } = Tags.Default;
    public Tag AttributeTag { get; set; } = Tags.Default;
    public float Magnitude { get; set; }
    public ModifierOperation Operation { get; set; }

    public float Calculate(Effect effect)
    {
        return MMC?.CalculateMagnitude(effect, Magnitude) ?? Magnitude;
    }

}