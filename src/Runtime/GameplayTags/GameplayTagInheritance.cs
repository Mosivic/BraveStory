using System;
using System.Collections.Generic;

public class GameplayTagInheritance
{
    // 存储标签之间的继承关系
    private readonly Dictionary<GameplayTag, HashSet<GameplayTag>> _inheritanceMap = new();
    
    // 存储标签的属性值
    private readonly Dictionary<GameplayTag, Dictionary<string, object>> _properties = new();
    
    // 添加继承关系
    public void AddInheritance(GameplayTag childTag, GameplayTag parentTag)
    {
        if (!_inheritanceMap.ContainsKey(childTag))
        {
            _inheritanceMap[childTag] = new HashSet<GameplayTag>();
        }
        _inheritanceMap[childTag].Add(parentTag);
    }
    
    // 移除继承关系
    public void RemoveInheritance(GameplayTag childTag, GameplayTag parentTag)
    {
        if (_inheritanceMap.TryGetValue(childTag, out var parents))
        {
            parents.Remove(parentTag);
            if (parents.Count == 0)
            {
                _inheritanceMap.Remove(childTag);
            }
        }
    }
    
    // 设置标签属性
    public void SetProperty<T>(GameplayTag tag, string propertyName, T value)
    {
        if (!_properties.ContainsKey(tag))
        {
            _properties[tag] = new Dictionary<string, object>();
        }
        _properties[tag][propertyName] = value;
    }
    
    // 获取标签属性（包括继承的属性）
    public bool TryGetProperty<T>(GameplayTag tag, string propertyName, out T value)
    {
        value = default;
        
        // 检查自身属性
        if (HasOwnProperty(tag, propertyName, out value))
        {
            return true;
        }
        
        // 检查继承的属性
        var parents = GetAllParentTags(tag);
        foreach (var parent in parents)
        {
            if (HasOwnProperty(parent, propertyName, out value))
            {
                return true;
            }
        }
        
        return false;
    }
    
    // 获取标签的所有父标签（包括间接父标签）
    private HashSet<GameplayTag> GetAllParentTags(GameplayTag tag)
    {
        var result = new HashSet<GameplayTag>();
        CollectParentTags(tag, result, new HashSet<GameplayTag>());
        return result;
    }
    
    // 递归收集父标签
    private void CollectParentTags(GameplayTag tag, HashSet<GameplayTag> result, HashSet<GameplayTag> visited)
    {
        if (!visited.Add(tag)) return; // 防止循环继承
        
        if (_inheritanceMap.TryGetValue(tag, out var parents))
        {
            foreach (var parent in parents)
            {
                result.Add(parent);
                CollectParentTags(parent, result, visited);
            }
        }
    }
    
    // 检查标签是否有自己的属性（不包括继承的属性）
    private bool HasOwnProperty<T>(GameplayTag tag, string propertyName, out T value)
    {
        value = default;
        
        if (_properties.TryGetValue(tag, out var properties) &&
            properties.TryGetValue(propertyName, out var objValue))
        {
            if (objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }
        }
        
        return false;
    }
    
    // 获取标签的所有属性（包括继承的属性）
    public Dictionary<string, object> GetAllProperties(GameplayTag tag)
    {
        var result = new Dictionary<string, object>();
        
        // 收集所有父标签的属性
        foreach (var parent in GetAllParentTags(tag))
        {
            if (_properties.TryGetValue(parent, out var parentProps))
            {
                foreach (var kvp in parentProps)
                {
                    if (!result.ContainsKey(kvp.Key))
                    {
                        result[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
        
        // 添加自身的属性（覆盖继承的属性）
        if (_properties.TryGetValue(tag, out var ownProps))
        {
            foreach (var kvp in ownProps)
            {
                result[kvp.Key] = kvp.Value;
            }
        }
        
        return result;
    }
    
    // 检查是否存在循环继承
    public bool HasCircularInheritance(GameplayTag tag)
    {
        return CheckCircularInheritance(tag, new HashSet<GameplayTag>());
    }
    
    private bool CheckCircularInheritance(GameplayTag tag, HashSet<GameplayTag> visited)
    {
        if (!visited.Add(tag))
        {
            return true; // 发现循环
        }
        
        if (_inheritanceMap.TryGetValue(tag, out var parents))
        {
            foreach (var parent in parents)
            {
                if (CheckCircularInheritance(parent, new HashSet<GameplayTag>(visited)))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
} 