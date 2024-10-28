using System;
using System.Collections.Generic;
using System.Linq;

public class GameplayTagQuery
{
    private List<GameplayTagRequirement> _anyRequirements = new();
    private List<GameplayTagRequirement> _allRequirements = new();
    private List<GameplayTagRequirement> _noneRequirements = new();
    
    public GameplayTagQuery AddAnyTag(GameplayTag tag, EGameplayTagMatchType matchType = EGameplayTagMatchType.Explicit)
    {
        _anyRequirements.Add(new GameplayTagRequirement(tag, matchType));
        return this;
    }
    
    public GameplayTagQuery AddAllTag(GameplayTag tag, EGameplayTagMatchType matchType = EGameplayTagMatchType.Explicit)
    {
        _allRequirements.Add(new GameplayTagRequirement(tag, matchType));
        return this;
    }
    
    public GameplayTagQuery AddNoneTag(GameplayTag tag, EGameplayTagMatchType matchType = EGameplayTagMatchType.Explicit)
    {
        _noneRequirements.Add(new GameplayTagRequirement(tag, matchType));
        return this;
    }
    
    public bool Matches(GameplayTagContainer container)
    {
        // 检查是否满足所有"必须包含"的标签
        if (_allRequirements.Count > 0 && !_allRequirements.All(req => container.HasTag(req.Tag, req.MatchType)))
            return false;
            
        // 检查是否满足"至少包含一个"的标签
        if (_anyRequirements.Count > 0 && !_anyRequirements.Any(req => container.HasTag(req.Tag, req.MatchType)))
            return false;
            
        // 检查是否不包含任何"禁止包含"的标签
        if (_noneRequirements.Any(req => container.HasTag(req.Tag, req.MatchType)))
            return false;
            
        return true;
    }
}

public readonly struct GameplayTagRequirement
{
    public readonly GameplayTag Tag;
    public readonly EGameplayTagMatchType MatchType;
    
    public GameplayTagRequirement(GameplayTag tag, EGameplayTagMatchType matchType)
    {
        Tag = tag;
        MatchType = matchType;
    }
}