using GPC;
using GPC.Job.Config;

public struct Condition<T>(Evaluator<T> evaluator, bool expect = true)
{
    private Evaluator<T> _evaluator = evaluator;
    private bool _expect = expect;
    
    public bool IsSatisfy(T state)
    {
        return _evaluator.Evaluate(state) == _expect;
    }
}