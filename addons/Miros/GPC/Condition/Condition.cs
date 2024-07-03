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

public class ConditionBase(CompareType type = CompareType.Equals)
{
    protected bool Involved { get; set; }
    protected ulong Checksum { get; set; }
}



public class BoolCondition(Func<bool> func, bool expect, CompareType type = CompareType.Equals):ConditionBase(type)
{

}

public class IntCondition(Func<int> func, int expect, CompareType type = CompareType.Equals):ConditionBase(type)
{
}

public class ttt
{
    private void CalcCompareResult<T>(T t)
    {
        var value = t.func.Invoke();
        switch (type)
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