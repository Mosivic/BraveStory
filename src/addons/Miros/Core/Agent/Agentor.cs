/*
    代理节点（AgentNode）是 Agent 的容器，用于初始化 Agent，同时管理自身下的所有状态节点（StateNode）。
    代理节点（AgentNode）与 宿主（Host）的区别是：代理节点 拥有 Agent 的访问权限，而 宿主 拥有所有子节点的访问权限，两者能够互相访问，
    但是代理节点无法直接访问宿主的子节点，宿主也无法直接访问代理节点的 Agent。

    状态节点可以与宿主共享数据，通过 Shared 实现。
*/

using System;
using Miros.Core;
using Godot;

namespace BraveStory;

[GodotClassName("AgentNode")]
public partial class AgentNode<THost,TAttributeSet,TShared> : AgentNodeBase
where THost : Node   
where TAttributeSet : AttributeSet
where TShared : Shared, new()
{   
    protected THost Host { get; private set; }
    public TShared Shared { get; private set; }

    public override void _Ready()
    {
        Host = Owner as THost;
        Agent.Initialize(Host as Node2D, [typeof(TAttributeSet)]);

        Shared = new TShared();
        HandleStateNodes(Shared);
    }

    private void HandleStateNodes(TShared shared)
    {
        var children = GetChildren();
		foreach (var child in children)
		{
            Console.WriteLine(child.GetType());
            if (child is not StateNode<State, THost,TShared>)
                continue;

            var stateNode = child as StateNode<State, THost,TShared>;
            HandleStateNode(stateNode,shared);
        }
    }

    private void HandleStateNode<TStateNode>(TStateNode stateNode,TShared shared) 
    where TStateNode : StateNode<State, THost,TShared>
    {
        stateNode.Initialize(Agent, Host,shared);
        AddStateFromNode(stateNode.ExecutorType, stateNode);
    }

    private void AddStateFromNode(ExecutorType executorType, StateNode<State, THost,TShared> stateNode)
    {
        switch (executorType)
        {
            case ExecutorType.MultiLayerStateMachine:
                Agent.AddState(executorType, stateNode.State, new StateFSMArgs(
                    stateNode.StateTag, stateNode.Transitions, stateNode.AnyTransition));
                break;
            case ExecutorType.EffectExecutor:
                Agent.AddState(executorType, stateNode.State);
                break;
        }
    }

    public override void _Process(double delta)
    {
        Agent.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        Agent.PhysicsProcess(delta);
    }

#pragma warning disable CS0693 // 类型参数与外部类型中的类型参数同名
    public override TShared GetShared<TShared>()
#pragma warning restore CS0693 // 类型参数与外部类型中的类型参数同名
    {
        return Shared as TShared;
    }
}
