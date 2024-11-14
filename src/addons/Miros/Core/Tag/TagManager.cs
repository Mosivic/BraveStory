using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class TagManager
{
    private static TagManager _instance;

    private readonly Dictionary<string, Tag> _registeredTags = new();
    private readonly Dictionary<Tag, HashSet<Tag>> _tagHierarchy = new();

    private TagManager()
    {
    }

    public static TagManager Instance
    {
        get
        {
            _instance ??= new TagManager();
            return _instance;
        }
    }

    public Tag RequestTag(string tagName)
    {
        if (string.IsNullOrEmpty(tagName)) return default;

        if (_registeredTags.TryGetValue(tagName, out var existingTag)) return existingTag;

        var newTag = new Tag(tagName);
        _registeredTags[tagName] = newTag;

        // 处理层级关系
        var parts = tagName.Split('.');
        if (parts.Length > 1)
        {
            var parentTagName = string.Join(".", parts.Take(parts.Length - 1));
            var parentTag = RequestTag(parentTagName);

            if (!_tagHierarchy.ContainsKey(parentTag)) _tagHierarchy[parentTag] = new HashSet<Tag>();
            _tagHierarchy[parentTag].Add(newTag);
        }

        return newTag;
    }


    // 检查 parentTag 是否是 tag 的父标签
    public bool IsParentOf(Tag tag, Tag parentTag)
    {
        if (!tag.IsValid || !parentTag.IsValid) return false;

        // 获取完整的标签路径
        var tagPath = tag.ToString();
        var parentPath = parentTag.ToString();

        // 如果父标签比子标签还长，显然不可能是父标签
        if (parentPath.Length >= tagPath.Length) return false;

        // 检查 tag 是否以 parentTag 开头，并且紧接着是分隔符
        return tagPath.StartsWith(parentPath) &&
               tagPath.Length > parentPath.Length &&
               tagPath[parentPath.Length] == '.';
    }

    // 检查 tag 是否是 parentTag 的子标签
    public bool IsChildOf(Tag tag, Tag parentTag)
    {
        return IsParentOf(tag, parentTag);
    }

    // 获取标签的所有父标签
    public IEnumerable<Tag> GetAllParentTags(Tag tag)
    {
        if (!tag.IsValid) yield break;

        var tagPath = tag.ToString();
        var parts = tagPath.Split('.');

        var currentPath = "";
        for (var i = 0; i < parts.Length - 1; i++)
        {
            if (i > 0) currentPath += ".";
            currentPath += parts[i];
            yield return RequestTag(currentPath);
        }
    }

    // 获取标签的直接子标签
    public IEnumerable<Tag> GetDirectChildTags(Tag parentTag)
    {
        if (!_tagHierarchy.TryGetValue(parentTag, out var children)) return Enumerable.Empty<Tag>();
        return children;
    }

    // 获取标签的所有子标签（包括子标签的子标签）
    public IEnumerable<Tag> GetAllChildTags(Tag parentTag)
    {
        var result = new HashSet<Tag>();
        CollectChildTags(parentTag, result);
        return result;
    }

    private void CollectChildTags(Tag parentTag, HashSet<Tag> result)
    {
        if (!_tagHierarchy.TryGetValue(parentTag, out var children)) return;

        foreach (var child in children)
        {
            result.Add(child);
            CollectChildTags(child, result);
        }
    }

    // 获取所有已注册的标签
    public IEnumerable<Tag> GetAllRegisteredTags()
    {
        return _registeredTags.Values;
    }

    // 获取标签的完整路径
    public string GetTagPath(Tag tag)
    {
        return _registeredTags.FirstOrDefault(x => x.Value == tag).Key ?? string.Empty;
    }

    // 检查标签是否已注册
    public bool IsTagRegistered(Tag tag)
    {
        return _registeredTags.ContainsValue(tag);
    }

    // 检查标签名是否已注册
    public bool IsTagNameRegistered(string tagName)
    {
        return _registeredTags.ContainsKey(tagName ?? string.Empty);
    }

    // 清除所有注册的标签（谨慎使用）
    public void ClearAllTags()
    {
        _registeredTags.Clear();
        _tagHierarchy.Clear();
    }

    // 重命名标签
    public bool RenameTag(string oldPath, string newPath)
    {
        if (string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath)) return false;

        // 检查新路径是否已存在
        if (_registeredTags.ContainsKey(newPath)) return false;

        // 获取旧标签
        if (!_registeredTags.TryGetValue(oldPath, out var oldTag)) return false;

        // 创建新标签
        var newTag = new Tag(newPath);

        // 更新注册表
        _registeredTags.Remove(oldPath);
        _registeredTags[newPath] = newTag;

        // 更新层级关系
        if (_tagHierarchy.TryGetValue(oldTag, out var children))
        {
            _tagHierarchy.Remove(oldTag);
            _tagHierarchy[newTag] = children;
        }

        // 更新父标签的子标签集合
        var oldParentPath = GetParentPath(oldPath);
        var newParentPath = GetParentPath(newPath);

        if (!string.IsNullOrEmpty(oldParentPath))
        {
            var oldParentTag = RequestTag(oldParentPath);
            if (_tagHierarchy.TryGetValue(oldParentTag, out var oldParentChildren)) oldParentChildren.Remove(oldTag);
        }

        if (!string.IsNullOrEmpty(newParentPath))
        {
            var newParentTag = RequestTag(newParentPath);
            if (!_tagHierarchy.ContainsKey(newParentTag)) _tagHierarchy[newParentTag] = new HashSet<Tag>();
            _tagHierarchy[newParentTag].Add(newTag);
        }

        return true;
    }

    // 获取标签的父路径
    private string GetParentPath(string tagPath)
    {
        if (string.IsNullOrEmpty(tagPath)) return string.Empty;

        var lastDotIndex = tagPath.LastIndexOf('.');
        if (lastDotIndex <= 0) return string.Empty;

        return tagPath.Substring(0, lastDotIndex);
    }

    // 获取标签的最后一个部分（名称）
    private string GetTagName(string tagPath)
    {
        if (string.IsNullOrEmpty(tagPath)) return string.Empty;

        var lastDotIndex = tagPath.LastIndexOf('.');
        if (lastDotIndex < 0) return tagPath;

        return tagPath.Substring(lastDotIndex + 1);
    }

    // 可选：添加一个公共方法来获取标签的父标签
    public Tag GetParentTag(Tag tag)
    {
        var tagPath = GetTagPath(tag);
        var parentPath = GetParentPath(tagPath);

        return string.IsNullOrEmpty(parentPath) ? default : RequestTag(parentPath);
    }
}