using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class TagContainer(HashSet<Tag> tags)
{
    private readonly HashSet<Tag> _tags = tags ?? [];

    // 定义事件
    public event EventHandler<TagEventArgs> TagAdded;
    public event EventHandler<TagEventArgs> TagRemoved;
    public event EventHandler<TagContainerEventArgs> TagsChanged;


    public bool HasTag(Tag tag)
    {
        return _tags.Contains(tag);
    }
    
    public bool HasAny(TagSet other)
    {
        return other.Empty || _tags.Overlaps(other.Tags);
    }
    
    public bool HasAny(TagContainer other)
    {
        return other != null && _tags.Overlaps(other._tags);
    }

    public bool HasAny(HashSet<Tag> other)
    {
        return other != null && _tags.Overlaps(other);
    }

    public bool HasAll(TagSet other)
    {
        return other.Empty || _tags.IsSupersetOf(other.Tags);
    }
    
    public bool HasAll(TagContainer other)
    {
        return other == null || _tags.IsSupersetOf(other._tags);
    }

    public bool HasAll(HashSet<Tag> other)
    {
        return other == null || _tags.IsSupersetOf(other);
    }


    public void AddTag(Tag tag)
    {
        if (!tag.IsValid || !_tags.Add(tag)) return;
        OnTagAdded(tag);
        OnTagsChanged([tag]);
    }

    public void RemoveTag(Tag tag)
    {
        if (!_tags.Remove(tag)) return;
        OnTagRemoved(tag);
        OnTagsChanged([tag]);
    }

    public void AppendTags(TagContainer other)
    {
        var addedTags = other._tags.Except(_tags).ToList();
        
        if (addedTags.Count == 0) return;
        _tags.UnionWith(addedTags);
        foreach (var tag in addedTags) OnTagAdded(tag);
        OnTagsChanged(addedTags);
    }

    public void RemoveTags(TagContainer other)
    {
        var removedTags = _tags.Intersect(other._tags).ToList();
        
        if (removedTags.Count == 0) return;
        _tags.ExceptWith(removedTags);
        foreach (var tag in removedTags) OnTagRemoved(tag);
        OnTagsChanged(removedTags);
    }

    public bool HasTagExact(Tag tag)
    {
        return _tags.Contains(tag);
    }

    public bool HasTag(Tag tag, TagMatchType matchType = TagMatchType.Explicit)
    {
        return matchType switch
        {
            TagMatchType.Explicit => HasTagExact(tag),
            TagMatchType.IncludeParentTags => HasTagOrParent(tag),
            TagMatchType.IncludeChildTags => HasTagOrChild(tag),
            _ => false
        };
    }

    public bool HasTagOrParent(Tag tag)
    {
        return _tags.Any(t =>
            t == tag || TagManager.Instance.IsParentOf(t, tag));
    }

    public bool HasTagOrChild(Tag tag)
    {
        return _tags.Any(t =>
            t == tag || TagManager.Instance.IsChildOf(t, tag));
    }

    public bool MatchesQuery(TagQuery query)
    {
        return query.Matches(this);
    }

    public IEnumerable<Tag> GetAllTags()
    {
        return _tags.ToList();
    }

    public HashSet<Tag> GetTags()
    {
        return _tags;
    }

    public int GetTagCount()
    {
        return _tags.Count;
    }

    private void OnTagAdded(Tag tag)
    {
        TagAdded?.Invoke(this, new TagEventArgs(tag, this));
    }

    private void OnTagRemoved(Tag tag)
    {
        TagRemoved?.Invoke(this, new TagEventArgs(tag, this));
    }

    private void OnTagsChanged(IEnumerable<Tag> tags)
    {
        TagsChanged?.Invoke(this, new TagContainerEventArgs(this, tags));
    }
}