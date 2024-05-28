using System;

namespace GPC;

public class Evaluator<T>(Predicate<T> predicate)
{
    public int VeriNum { get; set; } = 0;
    private Predicate<T> _predicate = predicate;

    public bool Evaluate(T t)
    {
       return predicate.Invoke(t);
    }
}
