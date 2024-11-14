/// <summary>
/// 属性集        
/// </summary>

namespace Miros.Core;

public abstract class AttributeSet
{
    public abstract AttributeBase this[string key] { get; }
    public abstract string[] AttributeNames { get; }
    public Persona Owner { get; private set; }

    /// <summary>
    ///     设置属性集的拥有者
    /// </summary>
    /// <param name="owner">拥有者</param>
    public void SetOwner(Persona owner)
    {
        Owner = owner;
        foreach (var attribute in AttributeNames) this[attribute].SetOwner(owner);
    }


    /// <summary>
    ///     修改属性基础值
    /// </summary>
    /// <param name="attributeShortName">属性名</param>
    /// <param name="value">新值</param>
    public void ChangeAttributeBase(string attributeShortName, float value)
    {
        if (this[attributeShortName] != null) this[attributeShortName].SetBaseValue(value);
    }
}