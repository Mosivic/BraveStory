namespace Miros.Core;

public class MultiLayerExecutorContext : Context
{
	public Tag Layer { get; private set; }
	public Transition[] Transitions { get; private set; }

	public MultiLayerExecutorContext(Tag layer, Transition[] transitions)
	{
		Layer = layer;
		Transitions = transitions;
	}

}