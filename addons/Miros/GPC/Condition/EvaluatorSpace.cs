namespace GPC;

public class EvaluatorSpace<G, L>(G Global, L local)
{
    public G Global { get; } = Global;
    public L Local { get; } = local;
}