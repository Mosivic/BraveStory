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
    public string AttributeName { get; set; } // 属性名称
    public string AttributeSetName { get; set; } // 属性集名称
    public string AttributeShortName { get; set; } // 属性短名称
    public float Magnitude { get; set; } // 幅度

    public ModifierOperation Operation { get; set; }

    public ModifierMagnitudeCalculation MMC; // 幅度计算

    
    public float CalculateMagnitude(Effect state, float modifierMagnitude)
    {
        return MMC == null ? Magnitude : MMC.CalculateMagnitude(state, modifierMagnitude);
    }


    void OnAttributeChanged()
    {
        var split = AttributeName.Split('.');
        AttributeSetName = split[0];
        AttributeShortName = split[1];

        // if (ReflectionHelper.GetAttribute(AttributeName)?.CalculateMode !=
        //     CalculateMode.Stacking)
        // {
        //     Operation = ModifierOperation.Override;
        // }
    }

}