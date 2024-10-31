using System;
using Godot;

namespace FSM.Evaluator;


public class Evaluator<T>(Func<T> func) : IEvaluator
    where T : IComparable
{
    private readonly Func<T> _func = func;
    private ulong _checksum;
    private T _value;
    private T _lastValue;
    private bool? _lastCompareResult;
    private T _lastCompareValue;
    private CompareType _lastCompareType;

    public string GetFuncValueString()
    {
        CalcFuncValue();
        return _value?.ToString() ?? "null";
    }

    public bool Is(T expectValue, CompareType type = CompareType.Equals)
    {
        CalcFuncValue();
        
        if (_lastCompareValue != null && 
            _lastCompareValue.Equals(expectValue) && 
            _lastCompareType == type && 
            _lastCompareResult.HasValue)
        {
            return _lastCompareResult.Value;
        }

        var result = Compare(_value, expectValue, type);
        
        _lastCompareValue = expectValue;
        _lastCompareType = type;
        _lastCompareResult = result;
        
        return result;
    }

    public bool LastIs(T expectValue, CompareType type = CompareType.Equals)
    {
        return Compare(_lastValue, expectValue, type);
    }

    private bool Compare(T value, T expectValue, CompareType type = CompareType.Equals)
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
        if (_checksum == frames)
            return;

        _lastValue = _value;
        _value = _func();
        _checksum = frames;
        
        _lastCompareResult = null;
    }

    public string GetLastValueString()
    {
        CalcFuncValue();
        return _lastValue?.ToString() ?? "null";
    }

}