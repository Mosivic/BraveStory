namespace Miros.Core;

public class MultiLayerSwitchArgs : Context
{
    public MultiLayerSwitchArgs(Tag layer, TransitionMode mode = TransitionMode.Force)
    {
        Layer = layer;
        Mode = mode;
    }

    public Tag Layer { get; private set; }
    public TransitionMode Mode { get; private set; }
}