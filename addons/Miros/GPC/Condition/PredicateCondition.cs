using System;
using GPC.Job.Config;

namespace GPC;

public class PredicateCondition(Predicate<IState> predicate) : ICondition
{
    public bool IsSatisfy()
    {
        throw new NotImplementedException();
    }

    public bool IsSatisfy(IState state)
    {
        return predicate.Invoke(state);
    }
}