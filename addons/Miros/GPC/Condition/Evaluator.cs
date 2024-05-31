using System;
using GPC.State;

namespace GPC;

public enum CompareFlags{
    Equal,
    Greater,
    Less
}

public interface IEvaluator{

}

public class Evaluator<S,R,CompareFlags,E>(Func<S, R> func,CompareFlags flag,E expect):IEvaluator
{
    public ulong Checksum { get; set; } = 0;
    private CompareFlags compareFlags {get;set;} = flag;
    private E Expect {get;set;} = expect;
    public R Result { get; set; }

    public R Evaluate(S state)
    {
        Result = func.Invoke(state);
        return Result;
    }
}