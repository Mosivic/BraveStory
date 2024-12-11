using Miros.Core;
using Godot;

namespace BraveStory;

public partial class AgentNode<THost,TAttributeSet> : Node 
where THost : Node
where TAttributeSet : AttributeSet
{   
    protected Agent Agent { get; private set; } = new();
    protected THost Host { get; private set; }

    public override void _Ready()
    {
        Host = GetParent<THost>();
        Agent.Initialize(Host as Node2D, [typeof(TAttributeSet)]);
    }


    private void HandleStateNodes()
    {
       	var children = GetChildren();
		foreach (var child in children)
		{
            if (child is not StateNode<State, THost>)
                continue;

            var stateNode = child as StateNode<State, THost>;
            HandleStateNode(stateNode);
        }
    }

    private void HandleStateNode<TStateNode>(TStateNode stateNode) 
    where TStateNode : StateNode<State, THost>
    {
        stateNode.Initialize(Agent, Host);
        AddStateFromNode(stateNode.ExecutorType, stateNode);
    }

    private void AddStateFromNode(ExecutorType executorType, StateNode<State, THost> stateNode)
    {
        switch (executorType)
        {
            // FIXME: 状态机未设置初始状态，当前无法运行
            case ExecutorType.MultiLayerStateMachine:
                Agent.AddState(executorType, stateNode.State, new StateFSMArgs(
                    stateNode.StateTag, stateNode.Transitions, stateNode.AnyTransition));
                break;
            case ExecutorType.EffectExecutor:
                Agent.AddState(executorType, stateNode.State);
                break;
        }
    }
}
