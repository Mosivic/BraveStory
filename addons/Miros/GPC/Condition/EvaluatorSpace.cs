using GPC.Job.Config;

namespace GPC;

public class EvaluatorSpace<S,C,P>(C common, P privato)
    where S : IState
    where C: EvaluatorLib<S>
    where P: EvaluatorLib<S>
{
    public C Common { get; } = common;
    public P Private { get; } = privato;
}