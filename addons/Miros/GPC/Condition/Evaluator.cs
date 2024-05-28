using System;
using GPC.Job.Config;

namespace GPC;

public class Evaluator(Func<IState, bool> func)
{
    private readonly Func<IState, bool> _func = func;
    public ulong Checksum { get; set; } = 666;
    public bool Result { get; set; }

    public bool Evaluate(IState state)
    {
        Result = _func.Invoke(state);
        return Result;
    }
}