using System;
using System.Collections.Generic;

namespace Miros.Core;

public enum AttributeValueType
{
    CurrentValue,
    BaseValue,
    MinValue,
    MaxValue
}

public class AttributeSetContainer(Agent owner)
{
    private static readonly Dictionary<Type, Tag> AttributeSetTypeMap = [];

    private readonly Dictionary<AttributeBase, AttributeAggregator> _attributeAggregators = [];

    private readonly Dictionary<string, AttributeBase> _attributes = []; // Lazy load attribute base

    public Dictionary<Tag, AttributeSet> Sets { get; } = [];


    #region AttributeBase Access

    public float Attribute(string attrName, AttributeValueType valueType = AttributeValueType.CurrentValue)
    {
        if (_attributes.TryGetValue(attrName, out var attr))
            return valueType switch
            {
                AttributeValueType.CurrentValue => attr.CurrentValue,
                AttributeValueType.BaseValue => attr.BaseValue,
                AttributeValueType.MinValue => attr.MinValue,
                AttributeValueType.MaxValue => attr.MaxValue,
                _ => throw new ArgumentException($"Invalid attribute value type: {valueType}")
            };
        if (TryGetAttributeBase(attrName, out attr))
        {
            _attributes.Add(attrName, attr);
            return Attribute(attrName, valueType);
        }

        throw new Exception($"Attribute {attrName} not found");
    }

    #endregion

    #region Tool Methods

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

    #endregion


    #region AttributeSet Management

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
        {
            var attr = attrSet.GetAttributeBase(tag);

            if (!_attributeAggregators.ContainsKey(attr))
            {
                var attrAggt = new AttributeAggregator(attr);
                attrAggt.OnEnable();
                _attributeAggregators.Add(attr, attrAggt);
            }
        }

        attrSet.Init(owner);
    }

    public void RemoveAttributeSet<T>() where T : AttributeSet
    {
        var setTag = AttributeSetTypeMap[typeof(T)];
        var attrSet = Sets[setTag];
        foreach (var tag in attrSet.AttributeTags) _attributeAggregators.Remove(attrSet.GetAttributeBase(tag));

        Sets.Remove(setTag);
    }

    #endregion

    #region AttributeSet Access

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

    #endregion

    #region AttributeBase Access

    public bool TryGetAttributeBase(Tag attrSetTag, Tag attrTag, out AttributeBase attr)
    {
        if (attrSetTag == Tags.Default)
        {
            if (TryGetAttributeBase(attrTag, out attr))
                return true;
        }
        else
        {
            if (Sets.TryGetValue(attrSetTag, out var set))
                if (set.TryGetAttribute(attrTag.ShortName, out attr))
                    return true;
        }

        attr = null;
        return false;
    }

    public bool TryGetAttributeBase(Tag attrTag, out AttributeBase attr)
    {
        foreach (var attrSet in Sets.Values)
            if (attrSet.TryGetAttribute(attrTag, out attr))
                return true;

        attr = null;
        return false;
    }


    public bool TryGetAttributeBase(string attrName, out AttributeBase attr, string attrSetName = "")
    {
        if (attrSetName != "")
            if (TryGetAttributeSet(attrSetName, out var set))
                if (set.TryGetAttribute(attrName, out attr))
                    return true;

        foreach (var attrSet in Sets)
            if (attrSet.Value.TryGetAttribute(attrName, out attr))
                return true;
        attr = null;
        return false;
    }

    #endregion

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