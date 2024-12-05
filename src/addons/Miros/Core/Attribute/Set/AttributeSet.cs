
using System;


/// <summary>
/// 属性集        
/// </summary>
namespace Miros.Core;

public abstract class AttributeSet
{
    public abstract AttributeBase[] Attributes { get; }

    public abstract Tag[] AttributeTags { get; }
    public abstract Tag AttributeSetTag { get; }
    public Agent Owner { get; private set; }

    /// <summary>
    ///     设置属性集的拥有者
    /// </summary>
    /// <param name="owner">拥有者</param>
    public void SetOwner(Agent owner)
    {
        Owner = owner;
        foreach (var attribute in Attributes) attribute.SetOwner(owner);
    }


    /// <summary>
    ///     修改属性基础值
    /// </summary>
    /// <param name="attributeSign">属性标签</param>
    /// <param name="value">新值</param>
    public void ChangeAttributeBase(Tag attributeSign, float value)
    {
        if (GetAttributeBase(attributeSign) != null) GetAttributeBase(attributeSign).SetBaseValue(value);
    }


    public AttributeBase GetAttributeBase(string tagName)
    {
        foreach (var attribute in Attributes)
        {
            if (attribute.AttributeTag.ShortName == tagName) return attribute;
        }
        throw new Exception($"AttributeSet {AttributeSetTag} does not contain attribute {tagName}");
    }

    public AttributeBase GetAttributeBase(Tag tag)
    {
        foreach (var attribute in Attributes)
        {
            if (attribute.AttributeTag == tag) return attribute;
        }
        throw new Exception($"AttributeSet {AttributeSetTag} does not contain attribute {tag.ShortName}");
    }
}
