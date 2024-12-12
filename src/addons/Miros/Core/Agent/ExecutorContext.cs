namespace Miros.Core;

public class ExecutorContext : Context
{
}

public class MultiLayerExecutorContext : ExecutorContext
{
    public MultiLayerExecutorContext(Tag layer, Transition[] transitions)
    {
        Layer = layer;
        Transitions = transitions;
    }

    public Tag Layer { get; private set; }
    public Transition[] Transitions { get; private set; }
}