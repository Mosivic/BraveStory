using System.Collections.Generic;

/// <summary>
/// 自定义属性集
/// 用于存储自定义的属性集
/// </summary>
namespace Miros.Core;

public class CustomAttrSet : AttributeSet
{
    private readonly Dictionary<string, AttributeBase> _attributes = [];

    public override AttributeBase this[string key] =>
        _attributes.TryGetValue(key, out var attribute) ? attribute : null;

    public override string[] AttributeNames { get; }

    public void AddAttribute(AttributeBase attribute)
    {
        if (_attributes.ContainsKey(attribute.Name))
            return;
        _attributes.Add(attribute.Name, attribute);
    }

    public void RemoveAttribute(AttributeBase attribute)
    {
        _attributes.Remove(attribute.Name);
    }
}