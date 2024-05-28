namespace GPC;

public class EvaluatorSpace<C, P>(C common, P privato)
{
    public C Common { get; } = common;
    public P Private { get; } = privato;
}