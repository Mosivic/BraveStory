namespace Miros.Core;

public enum ModifierMagnitudeType
{
    ScalableFloat
}

public struct ModifierMagnitude
{
    public ModifierMagnitudeType Type { get; set; }
}

public class Modifier(Tag attributeSetTag, Tag attributeTag, float magnitude, ModifierOperation operation)
{
    public ModifierMagnitudeCalculation MMC; // 幅度计算
    public Tag AttributeSetTag { get; set; } = attributeSetTag; // 属性集标签
    public Tag AttributeTag { get; set; } = attributeTag; // 属性标签
    public float Magnitude { get; set; } = magnitude; // 幅度
    public ModifierOperation Operation { get; set; } = operation; // 操作


    public float CalculateMagnitude(Effect effect)
    {
        return MMC?.CalculateMagnitude(effect, Magnitude) ?? Magnitude;
    }


    private void OnAttributeChanged()
    {
        // if (ReflectionHelper.GetAttribute(AttributeName)?.CalculateMode !=
        //     CalculateMode.Stacking)
        // {
        //     Operation = ModifierOperation.Override;
        // }
    }
}