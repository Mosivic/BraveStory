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

public struct AttributeIdentifier(Tag setTag, Tag tag)
{
    public Tag SetTag { get; set; } = setTag;
    public Tag Tag { get; set; } = tag;
}

public class Modifier(AttributeIdentifier attributeIdentifier, float magnitude, ModifierOperation operation)
{
    public ModifierMagnitudeCalculation MMC; // 幅度计算
    public AttributeIdentifier AttributeIdentifier { get; set; } = attributeIdentifier; // 属性标识符
    public float Magnitude { get; set; } = magnitude; // 幅度
    public ModifierOperation Operation { get; set; } = operation; // 操作


    public float CalculateMagnitude(Effect effect)
    {
        return MMC?.CalculateMagnitude(effect, Magnitude) ?? Magnitude;
    }
}