using System;
using System.Collections.Generic;

namespace Miros.Core;
public class TagFilter
{
    private List<FilterGroup> _filterGroups = new();
    
    public class FilterGroup
    {
        public List<FilterCondition> Conditions { get; } = new();
        public EFilterOperator GroupOperator { get; }
        
        public FilterGroup(EFilterOperator op = EFilterOperator.And)
        {
            GroupOperator = op;
        }
        
        // 添加链式调用支持
        public FilterGroup AddCondition(Tag tag, 
            TagMatchType matchType = TagMatchType.Explicit,
            EFilterType filterType = EFilterType.Include)
        {
            Conditions.Add(new FilterCondition(tag, matchType, filterType));
            return this;
        }
    }
    
    public class FilterCondition
    {
        public Tag Tag { get; }
        public TagMatchType MatchType { get; }
        public EFilterType FilterType { get; }
        
        public FilterCondition(Tag tag, TagMatchType matchType, EFilterType filterType)
        {
            Tag = tag;
            MatchType = matchType;
            FilterType = filterType;
        }
    }
    
    public enum EFilterType
    {
        Include,    // 必须包含
        Exclude,    // 必须不包含
        Any         // 至少包含一个
    }
    
    public enum EFilterOperator
    {
        And,        // 所有条件都必须满足
        Or          // 满足任一条件即可
    }
    
    // 创建新的过滤组
    public FilterGroup CreateGroup(EFilterOperator op = EFilterOperator.And)
    {
        var group = new FilterGroup(op);
        _filterGroups.Add(group);
        return group;
    }
    
    // 添加过滤条件到组
    public void AddCondition(FilterGroup group, Tag tag, 
        TagMatchType matchType = TagMatchType.Explicit,
        EFilterType filterType = EFilterType.Include)
    {
        group.Conditions.Add(new FilterCondition(tag, matchType, filterType));
    }
    
    // 执行过滤
    public bool Matches(TagContainer container)
    {
        // 如果没有过滤组，默认通过
        if (_filterGroups.Count == 0) return true;
        
        // 检查所有过滤组
        foreach (var group in _filterGroups)
        {
            bool groupResult = group.GroupOperator == EFilterOperator.And;
            
            foreach (var condition in group.Conditions)
            {
                bool matches = condition.FilterType switch
                {
                    EFilterType.Include => container.HasTag(condition.Tag, condition.MatchType),
                    EFilterType.Exclude => !container.HasTag(condition.Tag, condition.MatchType),
                    EFilterType.Any => container.HasTag(condition.Tag, condition.MatchType),
                    _ => false
                };
                
                if (group.GroupOperator == EFilterOperator.And)
                {
                    groupResult &= matches;
                    if (!groupResult) break; // 短路优化
                }
                else // Or
                {
                    groupResult |= matches;
                    if (groupResult) break; // 短路优化
                }
            }
            
            if (!groupResult) return false;
        }
        
        return true;
    }
    
    // 添加构建器模式接口
    public TagFilter WithGroup(EFilterOperator op = EFilterOperator.And, 
        Action<FilterGroup> groupBuilder = null)
    {
        var group = CreateGroup(op);
        groupBuilder?.Invoke(group);
        return this;
    }
    
    // 添加序列化支持
    public void SaveToJson(string path)
    {
        // 将过滤规则序列化为JSON
    }
    
    public static TagFilter LoadFromJson(string path)
    {
        // 从JSON加载过滤规则
        return null;
    }
    
    // 添加规则克隆
    public TagFilter Clone()
    {
        // 深度复制过滤规则
        return null;
    }
} 