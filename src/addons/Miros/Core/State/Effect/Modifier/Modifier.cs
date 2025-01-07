namespace Miros.Core;

public enum ModifierMagnitudeType
{
    ScalableFloat,
    AttributeBased,
    AttributeBasedWithStack,
    SetByCallerFromName,
    SetByCallerFromTag
}

public enum ModifierType
{
    Direct,
    Post,
}

public struct ModifierMagnitude
{
    public ModifierMagnitudeType Type { get; set; }
}

public class Modifier
{
    public ModifierMagnitudeCalculation MMC; // 幅度计算
    public ModifierType Type = ModifierType.Direct;


    public Modifier(Tag attributeTag, float magnitude, ModifierOperation operation,
        ModifierMagnitudeCalculation mmc,ModifierType type = ModifierType.Direct)
    {
        AttributeTag = attributeTag;
        Magnitude = magnitude;
        Operation = operation;
        MMC = mmc;
        Type = type;
    }

    public Modifier(Tag attributeSetTag, Tag attributeTag, float magnitude, ModifierOperation operation,
        ModifierMagnitudeCalculation mmc,ModifierType type = ModifierType.Direct)
    {
        AttributeSetTag = attributeSetTag;
        AttributeTag = attributeTag;
        Magnitude = magnitude;
        Operation = operation;
        MMC = mmc;
        Type = type;
    }


    public Tag AttributeSetTag { get; set; } = Tags.Default;
    public Tag AttributeTag { get; set; } = Tags.Default;
    public float Magnitude { get; set; }
    public ModifierOperation Operation { get; set; }
    public ModifierOperation PostOperation { get; set; } 


    public float CalculateMagnitude(Effect effect)
    {
        return MMC?.CalculateMagnitude(effect, Magnitude) ?? Magnitude;
    }
}