using System;
using GPC.State;

namespace GPC;

public class Evaluator(Func<IState, bool> func)
{
    public ulong Checksum { get; set; } = 0;
    public bool Result { get; set; }

    public bool Evaluate(IState state)
    {
        Result = func.Invoke(state);
        return Result;
    }
}