using System;
using System.Collections.Generic;

namespace Miros.Core;

public class AttributeSetContainer(Agent owner)
{
    private static readonly Dictionary<Type, Tag> AttributeSetTypeMap = [];
    private readonly Dictionary<AttributeBase, AttributeAggregator> _attributeAggregators = [];

    public Dictionary<Tag, AttributeSet> Sets { get; } = [];


    public void AddAttributeSet<T>() where T : AttributeSet
    {
        AddAttributeSet(typeof(T));
    }


    public void AddAttributeSet(Type attrSetType)
    {
        if (TryGetAttributeSet(attrSetType, out _)) return;

        var attrSet = Activator.CreateInstance(attrSetType) as AttributeSet;
        var attrSetTag = attrSet.AttributeSetTag;
        Sets.Add(attrSetTag, attrSet);
        AttributeSetTypeMap.Add(attrSetType, attrSetTag);

        foreach (var tag in attrSet.AttributeTags)
            if (!_attributeAggregators.ContainsKey(attrSet.GetAttributeBase(tag)))
            {
                var attrAggt = new AttributeAggregator(attrSet.GetAttributeBase(tag), owner);
                if (owner.Enabled) attrAggt.OnEnable();
                _attributeAggregators.Add(attrSet.GetAttributeBase(tag), attrAggt);
            }

        attrSet.SetOwner(owner);
    }

    public void RemoveAttributeSet<T>() where T : AttributeSet
    {
        var setTag = AttributeSetTypeMap[typeof(T)];
        var attrSet = Sets[setTag];
        foreach (var tag in attrSet.AttributeTags) _attributeAggregators.Remove(attrSet.GetAttributeBase(tag));

        Sets.Remove(setTag);
    }


    public bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet
    {
        if (Sets.TryGetValue(AttributeSetTypeMap[typeof(T)], out var set))
        {
            attributeSet = (T)set;
            return true;
        }

        attributeSet = null;
        return false;
    }

    public bool TryGetAttributeSet(Type attrSetType, out AttributeSet attributeSet)
    {
        if (AttributeSetTypeMap.TryGetValue(attrSetType, out var tag))
            if (Sets.TryGetValue(tag, out var set))
            {
                attributeSet = set;
                return true;
            }

        attributeSet = null;
        return false;
    }


    public bool TryGetAttributeSet(string attrSetName, out AttributeSet attributeSet)
    {
        foreach (var tag in Sets.Keys)
            // 注意！！！ 获取属性集如果使用字符串名称，会匹配三种情况
            // 1. 完全匹配，例如 传入字符串为"Character.Attack" 可以匹配标签为"Character.Attack"的属性集
            // 2. 短名称匹配，例如 传入字符串为"Attack" 可以匹配标签为"Character.Attack"的属性集
            // 3. 后代匹配，例如 传入字符串为"Character" 可以匹配标签为"Character.Attack"的属性集
            // 第二点和第三点会导致歧义，尽量使用完全匹配来获取属性集
            if (tag.FuallName == attrSetName || tag.ShortName == attrSetName || tag.IsDescendantOf(attrSetName))
            {
                attributeSet = Sets[tag];
                return true;
            }

        attributeSet = null;
        return false;
    }

    public AttributeIdentifier GetAttributeIdentifier(string attrSetName, string attrName)
    {
        if (TryGetAttributeSet(attrSetName, out var set))
            foreach (var attrTag in set.AttributeTags)
                if (attrTag.ShortName == attrName)
                    return new AttributeIdentifier(set.AttributeSetTag, attrTag);

        throw new Exception($"Attribute {attrName} not found in attribute set {attrSetName}");
    }

    public AttributeBase GetAttributeBase(Tag attrSetTag, Tag attrTag)
    {
        return Sets.TryGetValue(attrSetTag, out var set) ? set.GetAttributeBase(attrTag) : null;
    }


    public AttributeBase GetAttributeBase(string attrSetName, string attrName)
    {
        return TryGetAttributeSet(attrSetName, out var set) ? set.GetAttributeBase(attrName) : null;
    }


    public AttributeValue? GetAttributeValue(Tag attrSetTag, Tag attrTag)
    {
        return Sets.TryGetValue(attrSetTag, out var set)
            ? set.GetAttributeBase(attrTag).Value
            : null;
    }

    public AttributeValue? GetAttributeValue(string attrSetName, string attrName)
    {
        return TryGetAttributeSet(attrSetName, out var set)
            ? set.GetAttributeBase(attrName).Value
            : null;
    }

    public CalculateMode? GetAttributeCalculateMode(Tag attrSetSign, Tag attrSign)
    {
        return Sets.TryGetValue(attrSetSign, out var set)
            ? set.GetAttributeBase(attrSign).CalculateMode
            : null;
    }


    public float? GetAttributeBaseValue(Tag attrSetTag, Tag attrTag)
    {
        return Sets.TryGetValue(attrSetTag, out var set) ? set.GetAttributeBase(attrTag).BaseValue : null;
    }


    public float? GetAttributeBaseValue(string attrSetName, string attrName)
    {
        return TryGetAttributeSet(attrSetName, out var set) ? set.GetAttributeBase(attrName).BaseValue : null;
    }


    public float? GetAttributeCurrentValue(Tag attrSetTag, Tag attrTag)
    {
        return Sets.TryGetValue(attrSetTag, out var set)
            ? set.GetAttributeBase(attrTag).CurrentValue
            : null;
    }


    public float? GetAttributeCurrentValue(string attrSetName, string attrName)
    {
        return TryGetAttributeSet(attrSetName, out var set) ? set.GetAttributeBase(attrName).CurrentValue : null;
    }


    public Dictionary<Tag, float> Snapshot()
    {
        Dictionary<Tag, float> snapshot = [];
        foreach (var attributeSet in Sets)
        foreach (var tag in attributeSet.Value.AttributeTags)
        {
            var attr = attributeSet.Value.GetAttributeBase(tag);
            snapshot.Add(tag, attr.CurrentValue);
        }

        return snapshot;
    }

    /// <summary>
    /// 禁用所有属性集。
    /// </summary>
    // public void OnDisable()
    // {
    //     foreach (var aggregator in _attributeAggregators)
    //         aggregator.Value.OnDisable();
    // }

    /// <summary>
    /// 启用所有属性集。
    /// </summary>
    // public void OnEnable()
    // {
    //     foreach (var aggregator in _attributeAggregators)
    //         aggregator.Value.OnEnable();
    // }
}