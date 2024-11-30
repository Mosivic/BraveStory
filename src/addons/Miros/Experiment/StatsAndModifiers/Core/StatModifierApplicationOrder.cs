using System.Collections.Generic;
using System.Linq;

namespace Miros.Experiment.StatsAndModifiers;

public interface IStatModifierApplicationOrder
{
    int Apply(IEnumerable<StatModifier> statModifiers, int baseValue);
}

public class NormalStatModifierOrder : IStatModifierApplicationOrder
{
    public int Apply(IEnumerable<StatModifier> statModifiers, int baseValue)
    {
        var allModifier = statModifiers.ToList();

        foreach (var modifier in allModifier.Where(m => m.Strategy is AddOperationStrategy))
        {
            baseValue = modifier.Strategy.Calculate(baseValue);
        }

        foreach (var modifier in allModifier.Where(m => m.Strategy is MultiplyOperationStrategy))
        {
            baseValue =  modifier.Strategy.Calculate(baseValue);
        }

        return baseValue;
    }
}