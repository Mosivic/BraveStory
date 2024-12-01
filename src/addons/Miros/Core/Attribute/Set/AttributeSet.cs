/// <summary>
/// 属性集        
/// </summary>

namespace Miros.Core;

public abstract class AttributeSet
{
    public abstract AttributeBase this[Tag sign] { get; }
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
        foreach (var attribute in AttributeTags) this[attribute].SetOwner(owner);
    }


    /// <summary>
    ///     修改属性基础值
    /// </summary>
    /// <param name="attributeSign">属性标签</param>
    /// <param name="value">新值</param>
    public void ChangeAttributeBase(Tag attributeSign, float value)
    {
        if (this[attributeSign] != null) this[attributeSign].SetBaseValue(value);
    }
}