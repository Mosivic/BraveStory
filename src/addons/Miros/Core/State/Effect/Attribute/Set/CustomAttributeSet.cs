/// <summary>
/// 自定义属性集
/// 用于存储自定义的属性集
/// </summary>

namespace Miros.Core;

// public class CustomAttrSet : AttributeSet
// {
//     private readonly Dictionary<string, AttributeBase> _attributes = [];

//     public override AttributeBase this[Tag sign] =>
//         _attributes.TryGetValue(sign.Name, out var attribute) ? attribute : null;

//     public override Tag[] AttributeSigns => _attributes.Keys.Select(key => Tags.Attribute_ + key).ToArray();

//     public void AddAttribute(AttributeBase attribute)
//     {
//         if (_attributes.ContainsKey(attribute.Sign.Name))
//             return;
//         _attributes.Add(attribute.Sign.Name, attribute);
//     }

//     public void RemoveAttribute(AttributeBase attribute)
//     {
//         _attributes.Remove(attribute.Sign.Name);
//     }
// }