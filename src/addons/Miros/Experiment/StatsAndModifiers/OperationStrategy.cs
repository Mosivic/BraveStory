namespace Miros.Experiment.StatsAndModifiers;

public interface IOperationStrategy
{
    int Calculate(int value);
}

public class AddonOperationStrategy(int value) : IOperationStrategy
{
    private readonly int _value = value;

    public int Calculate(int value) => value + _value;
}

public class MultiplyOperationStrategy(int value) : IOperationStrategy
{
    private readonly int _value = value;
    
    public int Calculate(int value) => value * _value;
}