namespace Miros.Experiment.StatsAndModifiers;

public interface IOperationStrategy
{
    int Calculate(int value);
}

public class AddOperationStrategy(int value) : IOperationStrategy
{
    public int Calculate(int value1) => value1 + value;
}

public class MultiplyOperationStrategy(int value) : IOperationStrategy
{
    public int Calculate(int value1) => value1 * value;
}