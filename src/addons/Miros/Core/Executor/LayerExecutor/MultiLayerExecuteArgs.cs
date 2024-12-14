
namespace Miros.Core;

public class MultiLayerExecuteArgs : ExecuteArgs
{
    public MultiLayerExecuteArgs(Tag layer, Transition[] transitions)
    {
        Layer = layer;
        Transitions = transitions;
    }

    public MultiLayerExecuteArgs(Tag layer, Transition[] transitions, bool asDefaultTask = false)
    {
        Layer = layer;
        Transitions = transitions;
        AsDefaultTask = asDefaultTask;
    }

    public MultiLayerExecuteArgs(Tag layer, Transition[] transitions, bool asNextTask = false, TransitionMode asNextTaskTransitionMode = TransitionMode.Normal)
    {
        Layer = layer;
        Transitions = transitions;
        AsNextTask = asNextTask;
        AsNextTaskTransitionMode = asNextTaskTransitionMode;
    }
    

    public Tag Layer { get; private set; }
    public Transition[] Transitions { get; private set; }
    public bool AsNextTask { get; set; }
    public TransitionMode AsNextTaskTransitionMode { get; set; }
    public bool AsDefaultTask { get; set; }
}

public class MultiLayerSwitchTaskArgs : ExecuteArgs
{
    public MultiLayerSwitchTaskArgs(Tag layer, TransitionMode mode = TransitionMode.Force)
    {
        Layer = layer;
        Mode = mode;
    }

    public Tag Layer { get; private set; }
    public TransitionMode Mode { get; private set; }
}