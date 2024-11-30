using System;
using System.Collections.Generic;

namespace Miros.Core;

public class AttributeSetContainer(Agent owner)
{
    private readonly Dictionary<AttributeBase, AttributeAggregator> _attributeAggregators = [];

    private readonly Agent _owner = owner;
    public Dictionary<Tag, AttributeSet> Sets { get; } = [];


    /// <summary>
    ///     向容器中添加一个新的属性集。如果同类型的属性集已存在，则不会重复添加。
    /// </summary>
    /// <typeparam name="T">要添加的属性集类型，必须继承自AttributeSet</typeparam>
    public void AddAttributeSet<T>() where T : AttributeSet
    {
        AddAttributeSet(typeof(T));
    }

    /// <summary>
    ///     向容器中添加一个新的属性集。如果同类型的属性集已存在，则不会重复添加。
    /// </summary>
    /// <param name="attrSetType">要添加的属性集类型，必须继承自AttributeSet</param>
    public void AddAttributeSet(Type attrSetType)
    {
        if (TryGetAttributeSet(attrSetType, out _)) return;
        var setTag = AttributeSetUtil.AttributeSetTag(attrSetType);
        Sets.Add(setTag, Activator.CreateInstance(attrSetType) as AttributeSet);

        var attrSet = Sets[setTag];
        foreach (var attr in attrSet.AttributeSigns)
            if (!_attributeAggregators.ContainsKey(attrSet[attr]))
            {
                var attrAggt = new AttributeAggregator(attrSet[attr], _owner);
                // if (_owner.Enabled) attrAggt.OnEnable();
                _attributeAggregators.Add(attrSet[attr], attrAggt);
            }

        attrSet.SetOwner(_owner);
    }


    /// <summary>
    ///     从容器中移除一个属性集。
    /// </summary>
    /// <typeparam name="T">要移除的属性集类型，必须继承自AttributeSet</typeparam>
    public void RemoveAttributeSet<T>() where T : AttributeSet
    {
        var setTag = AttributeSetUtil.AttributeSetTag(typeof(T));
        var attrSet = Sets[setTag];
        foreach (var attr in attrSet.AttributeSigns) _attributeAggregators.Remove(attrSet[attr]);

        Sets.Remove(setTag);
    }

    /// <summary>
    ///     尝试获取一个属性集。
    /// </summary>
    /// <typeparam name="T">要获取的属性集类型，必须继承自AttributeSet</typeparam>
    /// <param name="attributeSet">获取到的属性集</param>
    /// <returns>是否成功获取到属性集</returns>
    public bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet
    {
        if (Sets.TryGetValue(AttributeSetUtil.AttributeSetTag(typeof(T)), out var set))
        {
            attributeSet = (T)set;
            return true;
        }

        attributeSet = null;
        return false;
    }

    /// <summary>
    ///     尝试获取一个属性集。
    /// </summary>
    /// <param name="attrSetType">要获取的属性集类型，必须继承自AttributeSet</param>
    /// <param name="attributeSet">获取到的属性集</param>
    public bool TryGetAttributeSet(Type attrSetType, out AttributeSet attributeSet)
    {
        if (Sets.TryGetValue(AttributeSetUtil.AttributeSetTag(attrSetType), out var set))
        {
            attributeSet = set;
            return true;
        }

        attributeSet = null;
        return false;
    }

    /// <summary>
    ///     获取一个属性集的属性值。
    /// </summary>
    /// <param name="attrSetSign">属性集标签</param>
    /// <param name="attrSign">属性标签</param>
    /// <returns>属性值</returns>
    public AttributeValue? GetAttributeAttributeValue(Tag attrSetSign, Tag attrSign)
    {
        return Sets.TryGetValue(attrSetSign, out var set)
            ? set[attrSign].Value
            : null;
    }

    /// <summary>
    ///     获取一个属性集的计算模式。
    /// </summary>
    /// <param name="attrSetSign">属性集标签</param>
    /// <param name="attrSign">属性标签</param>
    /// <returns>计算模式</returns>
    public CalculateMode? GetAttributeCalculateMode(Tag attrSetSign, Tag attrSign)
    {
        return Sets.TryGetValue(attrSetSign, out var set)
            ? set[attrSign].CalculateMode
            : null;
    }

    /// <summary>
    ///     获取一个属性集的基础值。
    /// </summary>
    /// <param name="attrSetSign">属性集标签</param>
    /// <param name="attrSign">属性标签</param>
    /// <returns>基础值</returns>
    public float? GetAttributeBaseValue(Tag attrSetSign, Tag attrSign)
    {
        return Sets.TryGetValue(attrSetSign, out var set) ? set[attrSign].BaseValue : null;
    }

    /// <summary>
    ///     获取一个属性集的当前值。
    /// </summary>
    /// <param name="attrSetSign">属性集标签</param>
    /// <param name="attrSign">属性标签</param>
    /// <returns>当前值</returns>
    public float? GetAttributeCurrentValue(Tag attrSetSign, Tag attrSign)
    {
        return Sets.TryGetValue(attrSetSign, out var set)
            ? set[attrSign].CurrentValue
            : null;
    }

    /// <summary>
    ///     获取一个属性集的快照。
    /// </summary>
    /// <returns>快照</returns>
    public Dictionary<Tag, float> Snapshot()
    {
        Dictionary<Tag, float> snapshot = [];
        foreach (var attributeSet in Sets)
            foreach (var sign in attributeSet.Value.AttributeSigns)
            {
                var attr = attributeSet.Value[sign];
                snapshot.Add(sign, attr.CurrentValue);
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