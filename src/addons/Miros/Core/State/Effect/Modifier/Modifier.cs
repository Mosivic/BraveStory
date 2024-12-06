namespace Miros.Core;

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


public class Modifier
{
    public ModifierMagnitudeCalculation MMC; // 幅度计算

    public Tag AttributeSetTag { get; set; }
    public Tag AttributeTag { get; set; }

    public float Magnitude { get; set; }
    public ModifierOperation Operation { get; set; }


    public Modifier(Tag attributeSetTag, Tag attributeTag, float magnitude, ModifierOperation operation)
    {
        AttributeSetTag = attributeSetTag;
        AttributeTag = attributeTag;
        Magnitude = magnitude;
        Operation = operation;
    }
    public float CalculateMagnitude(Effect effect)
    {
        return MMC?.CalculateMagnitude(effect, Magnitude) ?? Magnitude;
    }
}