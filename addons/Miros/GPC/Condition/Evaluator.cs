using System;
using GPC.State;

namespace GPC;

public class Evaluator<S,R>(Func<S, R> func)
{
    public ulong Checksum { get; set; } = 0;
    public R Result { get; set; }

    public R Evaluate(S state)
    {
        Result = func.Invoke(S);
        return Result;
    }
}