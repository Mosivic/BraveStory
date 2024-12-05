using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class AbsAgent
{
    protected readonly Dictionary<ExecutorType, IExecutor> Executors = [];
    protected readonly Node2D Host;

    protected readonly TagContainer OwnedTags;
    protected readonly StateExecutionRegistry StateExecutionRegistry = new();
    protected readonly ITaskProvider TaskProvider;


    public AbsAgent(Node2D host, ITaskProvider taskProvider)
    {
        Host = host;
        TaskProvider = taskProvider;
        OwnedTags = new TagContainer([]);
    }

    public bool Enabled { get; set; } = true;

    protected AttributeSetContainer AttributeSetContainer { get; set; }
}