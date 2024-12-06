namespace Miros.Core;

public class AttributeBasedModCalculation : ModifierMagnitudeCalculation
{
    public enum AttributeCaptureType
    {
        SnapShot, // 快照
        Track // 实时
    }

    /// <summary>
    ///     属性来源
    /// </summary>
    public enum AttributeFromType
    {
        Source, // 来源
        Target // 目标
    }

    public Tag attributeBasedSetTag; // 属性集的标签
    public Tag attributeBasedTag; // 属性的标签

    public AttributeFromType attributeFromType; // 属性来源

    public float b = 0; // 常量

    public AttributeCaptureType captureType; // 捕获方式

    public float k = 1; // 系数

    public override float CalculateMagnitude(Effect effect, float magnitude)
    {
        if (captureType == AttributeCaptureType.SnapShot)
        {
            var snapShot = attributeFromType == AttributeFromType.Source
                ? effect.SnapshotSourceAttributes
                : effect.SnapshotTargetAttributes;

            var attribute = snapShot[attributeBasedTag];
            return attribute * k + b;
        }
        else
        {
            var agent = attributeFromType == AttributeFromType.Source
                ? effect.Source
                : effect.Owner;

            var attribute = agent.GetAttributeBase(attributeBasedSetTag, attributeBasedTag);
            return (attribute?.CurrentValue ?? 1) * k + b;
        }
    }
}