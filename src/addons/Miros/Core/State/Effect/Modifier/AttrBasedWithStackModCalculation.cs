namespace Miros.Core;

/// <summary>
///     基于属性混合GE堆栈的MMC
/// </summary>
public class AttrBasedWithStackModCalculation : AttributeBasedModCalculation
{
    public enum StackMagnitudeOperation
    {
        Add,
        Multiply
    }


    public float sB = 0;

    /// <summary>
    ///     堆叠幅值计算
    ///     公式：StackCount * sK + sB
    /// </summary>
    public float sK = 1;


    public StackMagnitudeOperation stackMagnitudeOperation;

    public string FinalFormulae
    {
        get
        {
            var formulae = stackMagnitudeOperation switch
            {
                StackMagnitudeOperation.Add => $"({attributeBasedTag} * {k} + {b}) + (StackCount * {sK} + {sB})",
                StackMagnitudeOperation.Multiply => $"({attributeBasedTag} * {k} + {b}) * (StackCount * {sK} + {sB})",
                _ => ""
            };

            return $"<size=15><b><color=green>{formulae}</color></b></size>";
        }
    }

    public override float CalculateMagnitude(Effect effect, float magnitude)
    {
        var attrMagnitude = base.CalculateMagnitude(effect, magnitude);

        if (effect.Stacking.StackingType == StackingType.None) return attrMagnitude;

        var stackMagnitude = effect.Stacking.StackCount * sK + sB;

        return stackMagnitudeOperation switch
        {
            StackMagnitudeOperation.Add => attrMagnitude + stackMagnitude,
            StackMagnitudeOperation.Multiply => attrMagnitude * stackMagnitude,
            _ => attrMagnitude + stackMagnitude
        };
    }
}   