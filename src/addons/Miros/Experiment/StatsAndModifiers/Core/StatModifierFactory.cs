using System;

namespace Miros.Experiment.StatsAndModifiers;

public interface IStatModifierFactory
{
    StatModifier Create(IOperationStrategy operateType, StatType statType, int value, float duration);
}

public  class StatModifierFactory : IStatModifierFactory
{
    public StatModifier Create(IOperationStrategy operateType, StatType statType, int value, float duration)
    {
        IOperationStrategy strategy = operateType switch
        {
            AddOperationStrategy _ => new AddOperationStrategy(value),
            MultiplyOperationStrategy _ => new MultiplyOperationStrategy(value),
            _ => throw new ArgumentOutOfRangeException(nameof(operateType), operateType, null)
        };
        
        return new StatModifier(statType, strategy, duration);
    }
}