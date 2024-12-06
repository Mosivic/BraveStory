using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Miros.Core;

public class AbsAgent
{
    protected readonly Dictionary<ExecutorType, IExecutor> Executors = [];
    protected readonly Node2D Host;

    protected readonly TagContainer OwnedTags;
    protected readonly StateExecutionRegistry StateExecutionRegistry = new();
    protected readonly ITaskProvider TaskProvider;

    private readonly Dictionary<string, float> _attributes = [];

    public float Attr(string attrName)
    {
        if (_attributes.TryGetValue(attrName, out var value))
        {
            return value;
        } 
        else if (AttributeSetContainer.TryGetAttributeCurrentValue(attrName, out value))
        {
            _attributes[attrName] = value;
            return value;
        }

        throw new Exception($"Attribute {attrName} not found");
    }

    public void OnUpdateAttributes(string attrName,float value)
    {
        _attributes[attrName] = value;
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