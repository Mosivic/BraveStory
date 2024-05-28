using System;
using GPC.Job.Config;

namespace GPC;

public class PredicateCondition<T>(Predicate<T> predicate)  : ICondition<T> where T : IState
{
    public int CheckNum { get ; set ; } = 0;


    public bool IsSatisfy()
    {
        throw new NotImplementedException();
    }

    public bool IsSatisfy(T state)
    {
        return predicate.Invoke(state);
    }
}