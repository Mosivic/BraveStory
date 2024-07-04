using System;
using Godot;

namespace GPC.Evaluator;

public abstract class AbsEvaluator
{
}

public class Evaluator<T>(Func<T> func) : AbsEvaluator
    where T : IComparable
{
    private bool Involved { get; set; }
    private ulong Checksum { get; set; } 
    private Func<T> Func { get; } = func;


    public bool Invoke(T expectValue, CompareType type = CompareType.Equals)
    {
        var frames = Engine.GetProcessFrames();
        if (Checksum.Equals(frames))
            return Involved;

        var value = Func.Invoke();
        switch (type)
        {
            case CompareType.Equals:
                Involved = value.Equals(expectValue);
                break;
            case CompareType.Greater:
                Involved = value.CompareTo(expectValue) > 0;
                break;
            case CompareType.Less:
                Involved = value.CompareTo(expectValue) < 0;
                break;
        }

        Checksum = frames;
        return Involved;
    }
}