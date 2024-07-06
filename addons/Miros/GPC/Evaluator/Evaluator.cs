﻿using System;
using Godot;

namespace GPC.Evaluator;

public interface IEvaluator
{
    string GetFuncValueString();
}

public class Evaluator<T>(string name, Func<T> func) : IEvaluator
    where T : IComparable
{
    public string Name { get; set; } = name;
    private bool Result { get; set; }
    private ulong Checksum { get; set; } 
    private Func<T> Func { get; } = func;
    private T Value { get; set; }


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
    
    public string GetFuncValueString()
    {
        CalcFuncValue();
        return Value.ToString();
    }
}