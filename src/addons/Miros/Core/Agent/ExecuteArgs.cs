namespace Miros.Core;

public class ExecuteArgs : Context
{
}

public class MultiLayerExecuteArgs : ExecuteArgs
{
    public MultiLayerExecuteArgs(Tag layer, Transition[] transitions)
    {
        Layer = layer;
        Transitions = transitions;
    }

    public Tag Layer { get; private set; }
    public Transition[] Transitions { get; private set; }
}