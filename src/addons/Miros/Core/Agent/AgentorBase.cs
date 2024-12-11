/*
  作为AgentNode的基类，为了解决带有泛型的AgentNode初始化问题,并隐藏Agent的实现细节
*/
using System;
using Godot;
using Miros.Core;

namespace Miros.Core;

public abstract  class AgentorBase
{
    protected Agent Agent { get; private set; } = new();
    public abstract TShared GetShared<TShared>()
    where TShared : Shared, new();
    
    public void AddState(ExecutorType executorType,State state)
    {
        Agent.AddState(executorType,state);
    }

    public void RemoveState(ExecutorType executorType,State state)
    {
        Agent.RemoveState(executorType,state);
    }

    public void Throttle<T>(string eventName,EventHandler<T> handler) 
    where T : EventStreamArgs
    {
        Agent.EventStream.Throttle<T>(eventName,handler);
    }

    public void Unthrottle<T>(string eventName,EventHandler<T> handler)
    where T : EventStreamArgs
    {
        Agent.EventStream.Unthrottle<T>(eventName,handler);
    }

    public void Process(double delta)
    {
        Agent.Process(delta);
    }

    public void PhysicsProcess(double delta)
    {
        Agent.PhysicsProcess(delta);
    }
}