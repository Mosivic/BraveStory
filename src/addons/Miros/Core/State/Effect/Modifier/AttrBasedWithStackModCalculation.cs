

namespace Miros.Core;

/// <summary>
///  基于属性混合GE堆栈的MMC
/// </summary>
public class AttrBasedWithStackModCalculation : AttributeBasedModCalculation
{
    public enum StackMagnitudeOperation
    {
        Add,
        Multiply
    }

    /// <summary>
    /// 堆叠幅值计算
    /// 公式：StackCount * sK + sB
    /// </summary>
    public float sK = 1;


    public float sB = 0;


    public StackMagnitudeOperation stackMagnitudeOperation;

    public string FinalFormulae
    {
        get
        {
            var formulae = stackMagnitudeOperation switch
            {
                StackMagnitudeOperation.Add => $"({attributeName} * {k} + {b}) + (StackCount * {sK} + {sB})",
                StackMagnitudeOperation.Multiply => $"({attributeName} * {k} + {b}) * (StackCount * {sK} + {sB})",
                _ => ""
            };

            return $"<size=15><b><color=green>{formulae}</color></b></size>";
        }
    }

    public override float CalculateMagnitude(Effect spec, float modifierMagnitude)
    {
        var attrMagnitude = base.CalculateMagnitude(spec, modifierMagnitude);

        if (spec.Stacking.StackingType == StackingType.None) return attrMagnitude;

        var stackMagnitude = spec.StackCount * sK + sB;

        return stackMagnitudeOperation switch
        {
            StackMagnitudeOperation.Add => attrMagnitude + stackMagnitude,
            StackMagnitudeOperation.Multiply => attrMagnitude * stackMagnitude,
            _ => attrMagnitude + stackMagnitude
        };
    }

}
