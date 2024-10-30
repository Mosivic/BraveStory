using System;
using System.Collections.Generic;
using Godot;

public class GameplayTagInheritance
{
    private static GameplayTagInheritance _instance;
    public static GameplayTagInheritance Instance
    {
        get
        {
            _instance ??= new GameplayTagInheritance();
            return _instance;
        }
    }

    // 存储标签之间的继承关系
    private readonly Dictionary<GameplayTag, HashSet<GameplayTag>> _inheritanceMap = new();
    
    // 存储标签的属性值
    private readonly Dictionary<GameplayTag, Dictionary<string, object>> _properties = new();
    
    // 私有构造函数，防止外部直接实例化
    private GameplayTagInheritance() { }
    
    // 清理所有数据的方法（在需要重置时使用）
    public void Clear()
    {
        _inheritanceMap.Clear();
        _properties.Clear();
    }
    
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
    
    public T GetPropertyValue<T>(GameplayTag tag, string propertyName, T defaultValue)
    {
        // 检查当前标签是否有该属性
        if (_properties.TryGetValue(tag, out var properties) && 
            properties.TryGetValue(propertyName, out var value))
        {
            try
            {
                // 尝试将值转换为目标类型
                if (value is T typedValue)
                {
                    return typedValue;
                }
                // 处理数值类型之间的转换
                if (typeof(T).IsPrimitive && value is IConvertible)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch
            {
                GD.PrintErr($"Failed to convert property '{propertyName}' value to type {typeof(T)}");
            }
        }

        // 如果当前标签没有该属性，尝试从父标签继承
        if (_inheritanceMap.TryGetValue(tag, out var parents))
        {
            foreach (var parent in parents)
            {
                // 递归检查每个父标签
                var parentValue = GetPropertyValue(parent, propertyName, defaultValue);
                // 如果找到了非默认值，就返回它
                if (!EqualityComparer<T>.Default.Equals(parentValue, defaultValue))
                {
                    return parentValue;
                }
            }
        }

        // 如果找不到属性或转换失败，返回默认值
        return defaultValue;
    }
} 