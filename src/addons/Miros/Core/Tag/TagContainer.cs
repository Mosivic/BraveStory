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

    public bool HasAny(TagContainer other)
    {
        if (other == null) return false;
        return _tags.Overlaps(other._tags);
    }

    public bool HasAny(HashSet<Tag> other)
    {
        if (other == null) return false;
        return _tags.Overlaps(other);
    }

    public bool HasAll(TagContainer other)
    {
        if (other == null) return true;
        return _tags.IsSupersetOf(other._tags);
    }

    public bool HasAll(HashSet<Tag> other)
    {
        if (other == null) return true;
        return _tags.IsSupersetOf(other);
    }


    public void AddTag(Tag tag)
    {
        if (tag.IsValid && _tags.Add(tag))
        {
            OnTagAdded(tag);
            OnTagsChanged(new[] { tag });
        }
    }

    public void RemoveTag(Tag tag)
    {
        if (_tags.Remove(tag))
        {
            OnTagRemoved(tag);
            OnTagsChanged(new[] { tag });
        }
    }

    public void AppendTags(TagContainer other)
    {
        var addedTags = other._tags.Except(_tags).ToList();
        if (addedTags.Any())
        {
            _tags.UnionWith(addedTags);
            foreach (var tag in addedTags) OnTagAdded(tag);
            OnTagsChanged(addedTags);
        }
    }

    public void RemoveTags(TagContainer other)
    {
        var removedTags = _tags.Intersect(other._tags).ToList();
        if (removedTags.Any())
        {
            _tags.ExceptWith(removedTags);
            foreach (var tag in removedTags) OnTagRemoved(tag);
            OnTagsChanged(removedTags);
        }
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

    protected virtual void OnTagAdded(Tag tag)
    {
        TagAdded?.Invoke(this, new TagEventArgs(tag, this));
    }

    protected virtual void OnTagRemoved(Tag tag)
    {
        TagRemoved?.Invoke(this, new TagEventArgs(tag, this));
    }

    protected virtual void OnTagsChanged(IEnumerable<Tag> tags)
    {
        TagsChanged?.Invoke(this, new TagContainerEventArgs(this, tags));
    }
}