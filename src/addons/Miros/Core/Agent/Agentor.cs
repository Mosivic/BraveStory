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

public class Agentor<THost,TShared> : AgentorBase
where THost : Node   
where TShared : Shared, new()
{   
    public THost Host { get; private set; }
 
    public virtual void BindStators(THost host,TShared shared,Type[] stators)
    {
        Host = host;
        HandleStateNodes(shared,stators);
    }

    private void HandleStateNodes(TShared shared,Type[] stators)
    {
        foreach (var stator in stators)
        {
            var s = (Stator<State, THost, TShared>)Activator.CreateInstance(stator);
            HandleStateNode(s, shared);
        }
    }

    private void HandleStateNode<TStator>(TStator stator,TShared shared) 
    where TStator : Stator<State, THost,TShared>
    {
        stator.Initialize(Agent, Agent.Host as THost,shared);
        AddStateFromNode(stator.ExecutorType, stator);
    }

    private void AddStateFromNode(ExecutorType executorType, Stator<State, THost,TShared> stator)
    {
        switch (executorType)
        {
            case ExecutorType.MultiLayerStateMachine:
                Agent.AddState(executorType, stator.State, new StateFSMArgs(
                    stator.LayerTag, stator.Transitions, stator.AnyTransition));
                break;
            case ExecutorType.EffectExecutor:
                Agent.AddState(executorType, stator.State);
                break;
        }
    }
}
