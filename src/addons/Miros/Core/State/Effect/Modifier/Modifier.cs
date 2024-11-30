namespace Miros.Core;

public enum ModifierMagnitudeType
{
    ScalableFloat
}

public struct ModifierMagnitude
{
    public ModifierMagnitudeType Type { get; set; }
}

public class Modifier
{
    public ModifierMagnitudeCalculation MMC; // 幅度计算

    public Tag AttributeSetTag { get; set; } // 属性集标签
    public Tag AttributeTag { get; set; } // 属性标签
    public float Magnitude { get; set; } // 幅度

    public ModifierOperation Operation { get; set; }


    public float CalculateMagnitude(Effect state, float modifierMagnitude)
    {
        return MMC == null ? Magnitude : MMC.CalculateMagnitude(state, modifierMagnitude);
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