using System;
using System.Collections.Generic;
using Godot;
using GPC;
using GPC.States;

public enum CompareType
{
    Equals,
    Greater,
    Less
}

public interface ICondition<T>
{
    Evaluator<T> Evaluator { get; set; }
    T ExpectValue { get; set; }
    CompareType Type {get;set;}
}

public class ConditionBase
{
}

public class BoolCondition(Evaluator<bool> evaluator, bool expectValue, CompareType type = CompareType.Equals)
    : ConditionBase, ICondition<bool>
{
    public Evaluator<bool> Evaluator { get; set; } = evaluator;
    public bool ExpectValue { get; set; } = expectValue;
    public CompareType Type { get; set; } = type;
    
    private void Invoke()
    {
        bool value = Evaluator.Func.Invoke();
        Evaluator.Involved = value == ExpectValue;
    }
    
    public bool IsSatisfy()
    {
        var frames = Engine.GetProcessFrames();
        if (Evaluator.Checksum.Equals(frames))
            return Evaluator.Involved;

        Invoke();
        Evaluator.Checksum = frames;
        return Evaluator.Involved;
    }
}

public class Evaluator<T>(Func<T> func)
{
    public bool Involved { get; set; }
    public ulong Checksum { get; set; }
    public Func<T> Func { get; set; } = func;
    
    public void Invoke()
    {
        var value = Func.Invoke();
        switch (Type)
        {
            case CompareType.Equals:
                Involved = value == expect;
                break;
            case CompareType.Greater:
                Involved = value.CompareTo(expect) > 0;
                break;
            case CompareType.Less:
                Involved = value.CompareTo(expect) < 0;
                break;
        }
    }
}


public interface IEvaluatorFunc<T> 
{
    Func<T> Func{get;set;}
}

    public bool IsSatisfy()
    {
        var frames = Engine.GetProcessFrames();
        if (Checksum.Equals(frames))
            return Involved;

        CalcCompareResult();
        Checksum = frames;
        return Involved;
    }
}

public class ConditionA(Dictionary<Evaluator<object, object>, object> evaluators)
{
    public bool IsAllSatisfy(object state)
    {
        var frames = Engine.GetProcessFrames();
        foreach (var key in evaluators.Keys)
        {
            if (key.Checksum.Equals(frames) && key.Result != evaluators[key]) return false;

            if (key.Evaluate(state) != evaluators[key])
            {
                key.Checksum = frames;
                return false;
            }
        }

        return true;
    }

    public bool IsAnySatisfy(AbsState state)
    {
        var frames = Engine.GetProcessFrames();
        foreach (var key in evaluators.Keys)
        {
            if (key.Checksum.Equals(frames) && key.Result == evaluators[key]) return true;

            if (key.Evaluate(state) == evaluators[key])
            {
                key.Checksum = frames;
                return true;
            }
        }

        return false;
    }
}