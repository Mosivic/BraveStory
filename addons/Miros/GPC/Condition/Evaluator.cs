using System;
using GPC.States;

namespace GPC;

public enum CompareType
{
    Equals,
    Greater,
    Less
}

public interface IEvaluator<in S>
{
}


public class Evaluator(Func<object, object> func) : IEvaluator<object>
{

}

public class Evaluator<S>(Func<S, object> func) : IEvaluator<S>
{
    public ulong Checksum { get; set; }
    public object Result { get; set; }

    public virtual object Evaluate(S state)
    {
        Result = func.Invoke(state);
        return Result;
    }
}