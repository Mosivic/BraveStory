using System;
using System.Collections.Generic;
using System.Linq;
using BraveStory;

namespace Miros.Core;

public class GameplayTagContainer(HashSet<GameplayTag> tags)
{

    // 定义事件
    public event EventHandler<GameplayTagEventArgs> TagAdded;
    public event EventHandler<GameplayTagEventArgs> TagRemoved;
    public event EventHandler<GameplayTagContainerEventArgs> TagsChanged;
    

    private readonly HashSet<GameplayTag> _tags = tags;


    public bool HasTag(GameplayTag tag)
    {
        return _tags.Contains(tag);
    }
    
    public bool HasAny(GameplayTagContainer other)
    {
        if (other==null) return false;
        return _tags.Overlaps(other._tags);
    }
    public bool HasAny(HashSet<GameplayTag> other)
    {
        if (other==null) return false;
        return _tags.Overlaps(other);
    }
    
    public bool HasAll(GameplayTagContainer other)
    {
        if (other==null) return true;
        return _tags.IsSupersetOf(other._tags);
    }
    
    public bool HasAll(HashSet<GameplayTag> other)
    {
        if (other==null) return true;
        return _tags.IsSupersetOf(other);
    }


    public void AddTag(GameplayTag tag)
    {
        if (tag.IsValid && _tags.Add(tag))
        {
            OnTagAdded(tag);
            OnTagsChanged(new[] { tag });
        }
    }
    
    public void RemoveTag(GameplayTag tag)
    {
        if(_tags.Remove(tag)){
            OnTagRemoved(tag);
            OnTagsChanged(new[] { tag });
        }
    }
    
    public void AppendTags(GameplayTagContainer other)
    {
        var addedTags = other._tags.Except(_tags).ToList();
        if (addedTags.Any())
        {
            _tags.UnionWith(addedTags);
            foreach (var tag in addedTags)
            {
                OnTagAdded(tag);
            }
            OnTagsChanged(addedTags);
        }
    }

    public void RemoveTags(GameplayTagContainer other)
    {
        var removedTags = _tags.Intersect(other._tags).ToList();
        if (removedTags.Any())
        {
            _tags.ExceptWith(removedTags);
            foreach (var tag in removedTags)
            {
                OnTagRemoved(tag);
            }
            OnTagsChanged(removedTags);
        }
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
    
    public HashSet<GameplayTag> GetTags(){
        return _tags;
    }

    public int GetTagCount()
    {
        return _tags.Count;
    }

    protected virtual void OnTagAdded(GameplayTag tag)
    {
        TagAdded?.Invoke(this, new GameplayTagEventArgs(tag, this));
    }
    
    protected virtual void OnTagRemoved(GameplayTag tag)
    {
        TagRemoved?.Invoke(this, new GameplayTagEventArgs(tag, this));
    }
    
    protected virtual void OnTagsChanged(IEnumerable<GameplayTag> tags)
    {
        TagsChanged?.Invoke(this, new GameplayTagContainerEventArgs(this, tags));
    }
} 