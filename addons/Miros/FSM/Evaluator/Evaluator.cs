using System;
using Godot;

namespace FSM.Evaluator;

public interface IEvaluator
{
}

public class Evaluator<T>(Func<T> func) : IEvaluator
    where T : IComparable
{
    private bool Result { get; set; }
    private ulong Checksum { get; set; }
    private Func<T> Func { get; } = func;
    private T Value { get; set; }

    public string GetFuncValueString()
    {
        CalcFuncValue();
        return Value.ToString();
    }


    public bool Is(T expectValue, CompareType type = CompareType.Equals)
    {
        CalcFuncValue();

        switch (type)
        {
            case CompareType.Equals:
                Result = Value.Equals(expectValue);
                break;
            case CompareType.Greater:
                Result = Value.CompareTo(expectValue) > 0;
                break;
            case CompareType.Less:
                Result = Value.CompareTo(expectValue) < 0;
                break;
        }

        return Result;
    }

    private void CalcFuncValue()
    {
        var frames = Engine.GetProcessFrames();
        if (Checksum.Equals(frames))
            return;

        Value = Func.Invoke();
        Checksum = frames;
    }
    
}

public class Evaluator<T1,T2>( Func<T1,T2> func) : IEvaluator
    where T2 : IComparable
{
    private bool Result { get; set; }
    private ulong Checksum { get; set; }
    private Func<T1,T2> Func { get; } = func;
    private T2 Value { get; set; }

    public string GetFuncValueString(T1 @params)
    {
        CalcFuncValue(@params);
        return Value.ToString();
    }


    public bool Is(T1 @params,T2 expectValue, CompareType type = CompareType.Equals)
    {
        CalcFuncValue(@params);

        switch (type)
        {
            case CompareType.Equals:
                Result = Value.Equals(expectValue);
                break;
            case CompareType.Greater:
                Result = Value.CompareTo(expectValue) > 0;
                break;
            case CompareType.Less:
                Result = Value.CompareTo(expectValue) < 0;
                break;
        }

        return Result;
    }

    private void CalcFuncValue(T1 @params)
    {
        var frames = Engine.GetProcessFrames();
        if (Checksum.Equals(frames))
            return;

        Value = Func.Invoke(@params);
        Checksum = frames;
    }
    
}

