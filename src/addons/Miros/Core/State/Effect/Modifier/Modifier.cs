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


    public Modifier(Tag attributeTag, float magnitude, ModifierOperation operation,
        ModifierMagnitudeCalculation mmc, bool canGrab = true)
    {
        AttributeTag = attributeTag;
        Magnitude = magnitude;
        Operation = operation;
        MMC = mmc;
        CanGrab = canGrab;
    }

    public Modifier(Tag attributeSetTag, Tag attributeTag, float magnitude, ModifierOperation operation,
        ModifierMagnitudeCalculation mmc, bool canGrab = true)
    {
        AttributeSetTag = attributeSetTag;
        AttributeTag = attributeTag;
        Magnitude = magnitude;
        Operation = operation;
        MMC = mmc;
        CanGrab = canGrab;
    }

    public Tag AttributeSetTag { get; set; } = Tags.Default;
    public Tag AttributeTag { get; set; } = Tags.Default;
    public float Magnitude { get; set; }
    public ModifierOperation Operation { get; set; }

    public bool CanGrab { get; set; } = true;

    public float CalculateMagnitude(Effect effect)
    {
        return MMC?.CalculateMagnitude(effect, Magnitude) ?? Magnitude;
    }
}