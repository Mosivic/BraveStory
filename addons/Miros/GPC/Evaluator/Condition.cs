using System;

namespace GPC.Evaluator;


public enum CompareType
{
    Equals,
    Greater,
    Less
}

public interface ICondition<T>
where T : IComparable
{
    Evaluator<T> Evaluator { get; set; }
    T ExpectValue { get; set; }
    CompareType Type {get;set;}
}

public abstract class ConditionBase
{
    public abstract bool IsSatisfy();
}

public class BoolCondition(Evaluator<bool> evaluator, bool expectValue, CompareType type = CompareType.Equals)
    : ConditionBase, ICondition<bool>
{
    public Evaluator<bool> Evaluator { get; set; } = evaluator;
    public bool ExpectValue { get; set; } = expectValue;
    public CompareType Type { get; set; } = type;
    
    public override bool IsSatisfy()
    {
        return Evaluator.Invoke(ExpectValue,Type);
    }
}

public class FloatCondition(Evaluator<float> evaluator, float expectValue, CompareType type = CompareType.Equals)
    : ConditionBase, ICondition<float>
{
    public Evaluator<float> Evaluator { get; set; } = evaluator;
    public float ExpectValue { get; set; } = expectValue;
    public CompareType Type { get; set; } = type;
    
    public override bool IsSatisfy()
    {
        return Evaluator.Invoke(ExpectValue,Type);
    }
}