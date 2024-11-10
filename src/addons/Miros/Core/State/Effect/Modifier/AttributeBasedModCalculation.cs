namespace Miros.Core;

public class AttributeBasedModCalculation : ModifierMagnitudeCalculation
{
    public enum AttributeFrom
    {
        Source,

        Target
    }

    public enum GEAttributeCaptureType
    {
        SnapShot, // 快照
        Track // 实时
    }

    public GEAttributeCaptureType captureType; // 捕获方式

    public AttributeFrom attributeFromType; // 捕获目标

    public string attributeName; // 属性的名称

    public string attributeSetName; // 属性集名称

    public string attributeShortName;

    public float k = 1; // 系数

    public float b = 0; // 常量

    public override float CalculateMagnitude(Effect state, float modifierMagnitude)
    {
        if (attributeFromType == AttributeFrom.Source)
        {
            if (captureType == GEAttributeCaptureType.SnapShot)
            {
                var snapShot = state.SnapshotSourceAttributes;
                var attribute = snapShot[attributeName];
                return attribute * k + b;
            }
            else
            {
                var attribute = state.Source.GetAttributeCurrentValue(attributeSetName, attributeShortName);
                return (attribute ?? 1) * k + b;
            }
        }

        if (captureType == GEAttributeCaptureType.SnapShot)
        {
            var snapShot = state.SnapshotTargetAttributes;
            var attribute = snapShot[attributeName];
            return attribute * k + b;
        }
        else
        {
            var attribute = state.Owner.GetAttributeCurrentValue(attributeSetName, attributeShortName);
            return (attribute ?? 1) * k + b;
        }
    }

    private void OnAttributeNameChanged()
    {
        if (!string.IsNullOrWhiteSpace(attributeName))
        {
            var split = attributeName.Split('.');
            attributeSetName = split[0];
            attributeShortName = split[1];
        }
        else
        {
            attributeSetName = null;
            attributeShortName = null;
        }
    }
}
