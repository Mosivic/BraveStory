namespace Miros.Core;

public class AttributeBasedModCalculation : ModifierMagnitudeCalculation
{
    /// <summary>
    ///     属性来源
    /// </summary>
    public enum AttributeFrom
    {
        Source, // 来源
        Target // 目标
    }

    public enum EffectAttributeCaptureType
    {
        SnapShot, // 快照
        Track // 实时
    }

    public AttributeFrom attributeFromType; // 属性来源

    public Tag attributeSetSign; // 属性集的标签
    public Tag attributeSign; // 属性的标签

    public float b = 0; // 常量

    public EffectAttributeCaptureType captureType; // 捕获方式

    public float k = 1; // 系数

    public override float CalculateMagnitude(Effect effect, float modifierMagnitude)
    {
        if (attributeFromType == AttributeFrom.Source)
        {
            if (captureType == EffectAttributeCaptureType.SnapShot)
            {
                var snapShot = effect.SnapshotSourceAttributes;
                var attribute = snapShot[attributeSign];
                return attribute * k + b;
            }
            else
            {
                var attribute = effect.Source.GetAttributeCurrentValue(attributeSetSign, attributeSign);
                return (attribute ?? 1) * k + b;
            }
        }

        if (captureType == EffectAttributeCaptureType.SnapShot)
        {
            var snapShot = effect.SnapshotTargetAttributes;
            var attribute = snapShot[attributeSign];
            return attribute * k + b;
        }
        else
        {
            var attribute = effect.Owner.GetAttributeCurrentValue(attributeSetSign, attributeSign);
            return (attribute ?? 1) * k + b;
        }
    }

    private void OnAttributeNameChanged()
    {
    }
}