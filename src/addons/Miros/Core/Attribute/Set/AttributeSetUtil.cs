using System;
using System.Collections.Generic;


/// <summary>
/// 属性集工具类
/// </summary>
/// 用于缓存属性集类型和标签的映射关系
/// 并提供一个方法来获取属性集的标签
/// </summary>
namespace Miros.Core;

// public static class AttributeSetUtil
// {
//     public static Dictionary<Type, Tag> AttrSetTagCache { get; private set; }

//     /// <summary>
//     ///     缓存属性集类型和名称的映射关系
//     /// </summary>
//     /// <param name="typeToTag">类型和标签的映射关系</param>
//     public static void Cache(Dictionary<Type, Tag> typeToTag)
//     {
//         AttrSetTagCache = typeToTag;
//     }

//     /// <summary>
//     ///     获取属性集的标签
//     /// </summary>
//     /// <param name="attrSetType">属性集类型</param>
//     /// <returns>属性集标签</returns>
//     public static Tag AttributeSetTag(Type attrSetType)
//     {
//         if (AttrSetTagCache == null)
//             return new Tag(attrSetType.Name);

//         return AttrSetTagCache.TryGetValue(attrSetType, out var value) ? value : new Tag(attrSetType.Name);
//     }
// }