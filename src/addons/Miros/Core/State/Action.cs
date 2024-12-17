using System;
using Godot;

namespace Miros.Core;

public class Action<THost, TContext, TExecuteArgs> : State
    where THost : Node
    where TContext : Context
    where TExecuteArgs : ExecuteArgs
{
    public THost Host { get; private set; }
    public TContext Context { get; private set; }
    public virtual TExecuteArgs ExecuteArgs { get; private set; }

    public override Type TaskType => typeof(TaskBase<Action<THost, TContext, TExecuteArgs>>);


    public virtual void Init(THost host, TContext context, TExecuteArgs executeArgs)
    {
        Host = host;
        Context = context;
        ExecuteArgs = executeArgs;
    }
}