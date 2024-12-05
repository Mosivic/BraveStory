using System;
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

    public float Attr(string attrName)
    {
        if (AttributeSetContainer.TryGetAttributeCurrentValue(attrName, out var value))
            return value;
        throw new Exception($"Attribute {attrName} not found");
    }


    public AbsAgent(Node2D host, ITaskProvider taskProvider)
    {
        Host = host;
        TaskProvider = taskProvider;
        OwnedTags = new TagContainer([]);
    }

    public bool Enabled { get; set; } = true;

    protected AttributeSetContainer AttributeSetContainer { get; set; }
}