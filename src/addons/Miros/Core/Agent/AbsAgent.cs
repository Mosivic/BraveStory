using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Miros.Core;

public class AbsAgent
{
    protected readonly Dictionary<ExecutorType, IExecutor> Executors = [];
    protected readonly Node2D Host;
    protected AttributeSetContainer AttributeSetContainer { get; set; }
    protected readonly TagContainer OwnedTags;
    protected readonly StateExecutionRegistry StateExecutionRegistry = new();
    protected readonly ITaskProvider TaskProvider;

    private readonly Dictionary<string, float> _attributes = [];

    public bool Enabled { get; set; } = true;

	

    public float Attr(string attrName)
    {
        if (_attributes.TryGetValue(attrName, out var value))
        {
            return value;
        } 
        else if (AttributeSetContainer.TryGetAttributeBase(attrName,out var attr))
        {
            _attributes[attrName] = attr.CurrentValue;
			attr.UnregisterPostCurrentValueChange(OnUpdateAttributes);
            return attr.CurrentValue;
        }

        throw new Exception($"Attribute {attrName} not found");
    }


    public void OnUpdateAttributes(AttributeBase attr, float oldValue, float newValue)
    {
        _attributes[attr.AttributeTag.ShortName] = newValue;
    }



    public AbsAgent(Node2D host, ITaskProvider taskProvider)
    {
        Host = host;
        TaskProvider = taskProvider;
        OwnedTags = new TagContainer([]);
    }



    #region Tag Check

    public bool HasTag(Tag gameplayTag)
    {
        return OwnedTags.HasTag(gameplayTag);
    }

    public bool HasAll(TagSet tags)
    {
        return OwnedTags.HasAll(tags);
    }

    public bool HasAny(TagSet tags)
    {
        return OwnedTags.HasAny(tags);
    }

    #endregion	



    #region AttributeSet

    public AttributeIdentifier GetAttributeIdentifier(string attrSetName, string attrName)
    {
        return AttributeSetContainer.GetAttributeIdentifier(attrSetName, attrName);
    }

    public void AddAttributeSet(Type attrSetType)
    {
        AttributeSetContainer.AddAttributeSet(attrSetType);
    }

    public AttributeBase GetAttributeBase(Tag attrSetTag, Tag attrTag)
    {
        if (AttributeSetContainer.TryGetAttributeBase(attrSetTag, attrTag, out var value))
            return value;
        return null;
    }


    public Dictionary<Tag, float> DataSnapshot()
    {
        return AttributeSetContainer.Snapshot();
    }

    #endregion
}