using GPC;
using GPC.Job.Config;

public struct Condition(Evaluator<IState> evaluator, bool expect = true)
{
    private Evaluator<IState> _evaluator = evaluator;
    private bool _expect = expect;
    
    public bool IsSatisfy(IState state)
    {
        return _evaluator.Evaluate(state) == _expect;
    }
}