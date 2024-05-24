using System;
using GPC.Job.Config;

namespace GPC;

public class PredicateCondition : ICondition
{
    private readonly Predicate<State> _predicate;

    public PredicateCondition(Predicate<State> predicate)
    {
        _predicate = predicate;
    }

    public bool IsSatisfy()
    {
        throw new NotImplementedException();
    }

    public bool IsSatisfy(State cfg)
    {
        return _predicate.Invoke(cfg);
    }
}