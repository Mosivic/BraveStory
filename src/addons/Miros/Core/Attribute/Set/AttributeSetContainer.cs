using System;
using System.Collections.Generic;

namespace Miros.Core;

public class AttributeSetContainer(Persona owner)
{
    private readonly Persona _owner = owner;

    private readonly Dictionary<string, AttributeSet> _attributeSets = [];
    public Dictionary<string, AttributeSet> Sets => _attributeSets;

    private readonly Dictionary<AttributeBase, AttributeAggregator> _attributeAggregators = [];

    


    /// <summary>
    /// 向容器中添加一个新的属性集。如果同类型的属性集已存在，则不会重复添加。
    /// </summary>
    /// <typeparam name="T">要添加的属性集类型，必须继承自AttributeSet</typeparam>
    public void AddAttributeSet<T>() where T : AttributeSet
    {
        AddAttributeSet(typeof(T));
    }

    /// <summary>
    /// 向容器中添加一个新的属性集。如果同类型的属性集已存在，则不会重复添加。
    /// </summary>
    /// <param name="attrSetType">要添加的属性集类型，必须继承自AttributeSet</param>
    public void AddAttributeSet(Type attrSetType)
    {
        if (TryGetAttributeSet(attrSetType, out _)) return;
        var setName = AttributeSetUtil.AttributeSetName(attrSetType);
        _attributeSets.Add(setName, Activator.CreateInstance(attrSetType) as AttributeSet);

        var attrSet = _attributeSets[setName];
        foreach (var attr in attrSet.AttributeNames)
        {
            if (!_attributeAggregators.ContainsKey(attrSet[attr]))
            {
                var attrAggt = new AttributeAggregator(attrSet[attr], _owner);
                // if (_owner.Enabled) attrAggt.OnEnable();
                _attributeAggregators.Add(attrSet[attr], attrAggt);
            }
        }
        attrSet.SetOwner(_owner);
    }


    /// <summary>
    /// 从容器中移除一个属性集。
    /// </summary>
    /// <typeparam name="T">要移除的属性集类型，必须继承自AttributeSet</typeparam>
    public void RemoveAttributeSet<T>() where T : AttributeSet
    {
        var setName = AttributeSetUtil.AttributeSetName(typeof(T));
        var attrSet = _attributeSets[setName];
        foreach (var attr in attrSet.AttributeNames)
        {
            _attributeAggregators.Remove(attrSet[attr]);
        }

        _attributeSets.Remove(setName);
    }

    /// <summary>
    /// 尝试获取一个属性集。
    /// </summary>
    /// <typeparam name="T">要获取的属性集类型，必须继承自AttributeSet</typeparam>
    /// <param name="attributeSet">获取到的属性集</param>
    /// <returns>是否成功获取到属性集</returns>
    public bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet
    {
        if (_attributeSets.TryGetValue(AttributeSetUtil.AttributeSetName(typeof(T)), out var set))
        {
            attributeSet = (T)set;
            return true;
        }

        attributeSet = null;
        return false;
    }

    /// <summary>
    /// 尝试获取一个属性集。
    /// </summary>
    /// <param name="attrSetType">要获取的属性集类型，必须继承自AttributeSet</param>
    /// <param name="attributeSet">获取到的属性集</param>
    public bool TryGetAttributeSet(Type attrSetType, out AttributeSet attributeSet)
    {
        if (_attributeSets.TryGetValue(AttributeSetUtil.AttributeSetName(attrSetType), out var set))
        {
            attributeSet = set;
            return true;
        }

        attributeSet = null;
        return false;
    }

    /// <summary>
    /// 获取一个属性集的属性值。
    /// </summary>
    /// <param name="attrSetName">属性集名称</param>
    /// <param name="attrShortName">属性名</param>
    /// <returns>属性值</returns>
    public AttributeValue? GetAttributeAttributeValue(string attrSetName, string attrShortName)
    {
        return _attributeSets.TryGetValue(attrSetName, out var set)
            ? set[attrShortName].Value
            : null;
    }

    /// <summary>
    /// 获取一个属性集的计算模式。
    /// </summary>
    /// <param name="attrSetName">属性集名称</param>
    /// <param name="attrShortName">属性名</param>
    /// <returns>计算模式</returns>
    public CalculateMode? GetAttributeCalculateMode(string attrSetName, string attrShortName)
    {
        return _attributeSets.TryGetValue(attrSetName, out var set)
            ? set[attrShortName].CalculateMode
            : (CalculateMode?)null;
    }

    /// <summary>
    /// 获取一个属性集的基础值。
    /// </summary>
    /// <param name="attrSetName">属性集名称</param>
    /// <param name="attrShortName">属性名</param>
    /// <returns>基础值</returns>
    public float? GetAttributeBaseValue(string attrSetName, string attrShortName)
    {
        return _attributeSets.TryGetValue(attrSetName, out var set) ? set[attrShortName].BaseValue : (float?)null;
    }

    /// <summary>
    /// 获取一个属性集的当前值。
    /// </summary>
    /// <param name="attrSetName">属性集名称</param>
    /// <param name="attrShortName">属性名</param>
    /// <returns>当前值</returns>
    public float? GetAttributeCurrentValue(string attrSetName, string attrShortName)
    {
        return _attributeSets.TryGetValue(attrSetName, out var set)
            ? set[attrShortName].CurrentValue
            : (float?)null;
    }

    /// <summary>
    /// 获取一个属性集的快照。
    /// </summary>
    /// <returns>快照</returns>
    public Dictionary<string, float> Snapshot()
    {
        Dictionary<string, float> snapshot = [];
        foreach (var attributeSet in _attributeSets)
        {
            foreach (var name in attributeSet.Value.AttributeNames)
            {
                var attr = attributeSet.Value[name];
                snapshot.Add(attr.Name, attr.CurrentValue);
            }
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
