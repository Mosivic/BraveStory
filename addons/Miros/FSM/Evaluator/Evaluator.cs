using System;
using Godot;

namespace FSM.Evaluator;

public interface IEvaluator
{
}

public class Evaluator<T>(Func<T> func) : IEvaluator
    where T : IComparable
{
    private ulong Checksum { get; set; }
    private Func<T> Func { get; } = func;
    private T Value { get; set; }
    private T LastValue { get; set; }

    public string GetFuncValueString()
    {
        CalcFuncValue();
        return Value.ToString();
    }


    public bool Is(T expectValue, CompareType type = CompareType.Equals)
    {
        return Compare(Value, expectValue, type);
    }


    public bool LastIs(T expectValue, CompareType type = CompareType.Equals)
    {
        return Compare(LastValue, expectValue, type);
    }

    private bool Compare(T value,T expectValue, CompareType type = CompareType.Equals)
    {
        CalcFuncValue();

        switch (type)
        {
            case CompareType.Equals:
                return value.Equals(expectValue);
            case CompareType.Greater:
                return value.CompareTo(expectValue) > 0;
            case CompareType.Less:
                return value.CompareTo(expectValue) < 0;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private void CalcFuncValue()
    {
        var frames = Engine.GetProcessFrames();
        if (Checksum.Equals(frames))
            return;

        LastValue = Value;
        Value = Func.Invoke();
        Checksum = frames;
    }
}
