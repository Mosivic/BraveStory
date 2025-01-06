using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class Agent
{
    private TagContainer OwnedTags;
    private AttributeSetContainer AttributeSetContainer { get; set; }
    private StateDispatcher StateDispatcher { get; set; }

    public Node Host { get; private set; }
    public bool Enabled { get; private set; }
    public EventStream EventStream { get; private set; }


    public void Init(Node host)
    {
        Enabled = true;
        Host = host;
        EventStream = new EventStream();

        OwnedTags = new TagContainer([]);
        AttributeSetContainer = new AttributeSetContainer(this);
        StateDispatcher = new StateDispatcher(this);
    }


    public void Process(double delta)
    {
        if (Enabled)
            StateDispatcher.Update(delta);
    }


    public void PhysicsProcess(double delta)
    {
        if (Enabled)
            StateDispatcher.PhysicsUpdate(delta);
    }


    public float Atr(string attrName, AttributeValueType valueType = AttributeValueType.CurrentValue)
    {
        return AttributeSetContainer.Attribute(attrName, valueType);
    }

    public void AddState(State state, Context context = null)
    {
        StateDispatcher.AddState(state, context);
    }


    public void SwitchTaskByTag(Tag tag, Context switchArgs = null)
    {
        StateDispatcher.SwitchTaskByTag(tag, switchArgs);
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

    public AttributeBase GetAttributeBase(string attrName, string attrSetName = "")
    {
        if (AttributeSetContainer.TryGetAttributeBase(attrName, out var value, attrSetName))
            return value;
        return null;
    }


    public Dictionary<Tag, float> DataSnapshot()
    {
        return AttributeSetContainer.Snapshot();
    }

    #endregion
}