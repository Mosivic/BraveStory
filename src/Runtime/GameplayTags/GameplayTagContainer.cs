using System.Collections.Generic;
using System.Linq;

public class GameplayTagContainer
{
    private readonly HashSet<GameplayTag> _tags = new();
    
    public bool HasTag(GameplayTag tag)
    {
        return _tags.Contains(tag);
    }
    
    public bool HasAny(GameplayTagContainer other)
    {
        return _tags.Overlaps(other._tags);
    }
    
    public bool HasAll(GameplayTagContainer other)
    {
        return _tags.IsSupersetOf(other._tags);
    }
    
    public void AddTag(GameplayTag tag)
    {
        if (tag.IsValid)
        {
            _tags.Add(tag);
        }
    }
    
    public void RemoveTag(GameplayTag tag)
    {
        _tags.Remove(tag);
    }
    
    public void AppendTags(GameplayTagContainer other)
    {
        _tags.UnionWith(other._tags);
    }

    public bool HasTagExact(GameplayTag tag)
    {
        return _tags.Contains(tag);
    }
    
    public bool HasTag(GameplayTag tag, EGameplayTagMatchType matchType = EGameplayTagMatchType.Explicit)
    {
        return matchType switch
        {
            EGameplayTagMatchType.Explicit => HasTagExact(tag),
            EGameplayTagMatchType.IncludeParentTags => HasTagOrParent(tag),
            EGameplayTagMatchType.IncludeChildTags => HasTagOrChild(tag),
            _ => false
        };
    }
    
    public bool HasTagOrParent(GameplayTag tag)
    {
        return _tags.Any(t => 
            t == tag || GameplayTagManager.Instance.IsParentOf(t, tag));
    }
    
    public bool HasTagOrChild(GameplayTag tag)
    {
        return _tags.Any(t => 
            t == tag || GameplayTagManager.Instance.IsChildOf(t, tag));
    }
    
    public bool MatchesQuery(GameplayTagQuery query)
    {
        return query.Matches(this);
    }
    
    public IEnumerable<GameplayTag> GetAllTags()
    {
        return _tags.ToList();
    }
    
    public int GetTagCount()
    {
        return _tags.Count;
    }
} 