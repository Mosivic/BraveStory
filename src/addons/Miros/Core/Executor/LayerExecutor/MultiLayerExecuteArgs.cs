
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

    public MultiLayerExecuteArgs(Tag layer, Transition[] transitions, bool asCurrentTask = false, TransitionMode asCurrentTaskTransitionMode = TransitionMode.Normal)
    {
        Layer = layer;
        Transitions = transitions;
        AsCurrentTask = asCurrentTask;
    }

    public Tag Layer { get; private set; }
    public Transition[] Transitions { get; private set; }
    public bool AsCurrentTask { get; set; }
    public TransitionMode AsCurrentTaskTransitionMode { get; set; }
    public bool AsDefaultTask { get; set; }
}