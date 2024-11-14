using System;
using System.Collections.Generic;


/// <summary>
/// 属性集工具类
/// </summary>
/// 用于缓存属性集类型和名称的映射关系
/// 并提供一个方法来获取属性集的名称
/// </summary>
namespace Miros.Core;

public static class AttributeSetUtil
{
    public static Dictionary<Type, string> AttrSetNameCache { get; private set; }

    /// <summary>
    /// 缓存属性集类型和名称的映射关系
    /// </summary>
    /// <param name="typeToName">类型和名称的映射关系</param>
    public static void Cache(Dictionary<Type, string> typeToName)
    {
        AttrSetNameCache = typeToName;
    }

    /// <summary>
    /// 获取属性集的名称
    /// </summary>
    /// <param name="attrSetType">属性集类型</param>
    /// <returns>属性集名称</returns>
    public static string AttributeSetName(Type attrSetType)
    {
        if (AttrSetNameCache == null)
            return attrSetType.Name;

        return AttrSetNameCache.TryGetValue(attrSetType, out var value) ? value : attrSetType.Name;
    }
}