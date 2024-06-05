using System;
using System.Collections.Generic;
using Godot;
using GPC;
using GPC.States;



public class Condition(Func<> func,object expect, CompareType type = CompareType.Equals)
{
    private bool Involved { get; set; }
    private ulong Checksum { get; set; }
    private object Value { get; set; }
    private void CalcCompareResult(object state)
    {
        Value = func.Invoke(state);
        switch (type)
        {
            case CompareType.Equals:
                Involved = Value == expect;
                break;
            case CompareType.Greater:
                Involved = ((IComparable)Value).CompareTo(expect as IComparable) > 0;
                break;
            case CompareType.Less:
                Involved = ((IComparable)Value).CompareTo(expect as IComparable) < 0;
                break;
        }
    }
    public bool IsSatisfy(object state)
    {
        var frames = Engine.GetProcessFrames();
        if (Checksum.Equals(frames))
            return Involved;

        CalcCompareResult(state);
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

    public bool IsAnySatisfy(IState state)
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