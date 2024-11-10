using System;
using System.Collections.Generic;
using Miros.Utils;

namespace Miros.Core;

public class TagAggregator
{
    private Persona _owner;

    private Dictionary<Tag, List<object>> _dynamicAddedTags = new();

    private Dictionary<Tag, List<object>> _dynamicRemovedTags = new();

    private readonly List<Tag> _fixedTags = new();

    private static Pool _pool = new Pool(typeof(List<object>), 1024);

    public TagAggregator(Persona owner)
    {
        _owner = owner;
    }

    private event Action OnTagIsDirty;

    private void TagIsDirty(TagSet tags)
    {
        if (!tags.Empty) OnTagIsDirty?.Invoke();
    }

    private void TagIsDirty(Tag tag)
    {
        OnTagIsDirty?.Invoke();
    }

    public void Init(Tag[] tags)
    {
        _fixedTags.Clear();
        _fixedTags.AddRange(tags);
    }

    public void OnEnable()
    {
        // 有 GC, 无法避免
        OnTagIsDirty += _owner.EffectContainer.RefreshEffectStatus;
    }

    public void OnDisable()
    {
        OnTagIsDirty -= _owner.EffectContainer.RefreshEffectStatus;
    }


    private static bool IsTagInList(Tag tag, List<Tag> tags)
    {
        foreach (var t in tags)
        {
            if (t == tag)
            {
                return true;
            }
        }

        return false;
    }

    private bool TryAddFixedTag(Tag tag)
    {
        var dirty = !IsTagInList(tag, _fixedTags);
        if (dirty) _fixedTags.Add(tag);
        var dynamicRemovedTagsRemoved = _dynamicRemovedTags.Remove(tag);
        dirty = dirty || dynamicRemovedTagsRemoved;
        var dynamicAddedTagsRemoved = _dynamicAddedTags.Remove(tag);
        dirty = dirty || dynamicAddedTagsRemoved;
        return dirty;
    }

    public void AddFixedTag(Tag tag)
    {
        var dirty = TryAddFixedTag(tag);
        if (dirty) TagIsDirty(tag);
    }

    public void AddFixedTag(TagSet tagSet)
    {
        if (tagSet.Empty) return;
        var dirty = false;
        foreach (var tag in tagSet.Tags) dirty = dirty || TryAddFixedTag(tag);

        if (dirty) TagIsDirty(tagSet);
    }

    private bool TryRemoveFixedTag(Tag tag)
    {
        var dirty = _fixedTags.Remove(tag);
        var dynamicAddedTagsRemoved = _dynamicAddedTags.Remove(tag);
        dirty = dirty || dynamicAddedTagsRemoved;
        var dynamicRemovedTagsRemoved = _dynamicRemovedTags.Remove(tag);
        dirty = dirty || dynamicRemovedTagsRemoved;
        return dirty;
    }

    public void RemoveFixedTag(Tag tag)
    {
        var dirty = TryRemoveFixedTag(tag);
        if (dirty) TagIsDirty(tag);
    }

    public void RemoveFixedTag(TagSet tagSet)
    {
        if (tagSet.Empty) return;
        var dirty = false;
        foreach (var tag in tagSet.Tags) dirty = dirty || TryRemoveFixedTag(tag);

        if (dirty) TagIsDirty(tagSet);
    }

    private bool TryAddDynamicAddedTag<T>(T source, Tag tag)
    {
        if (!(source is Effect) && !(source is Ability))
        {
            return false;
        }

        var dirty = _dynamicRemovedTags.Remove(tag);
        foreach (var t in _fixedTags)
        {
            if (t == tag)
            {
                return dirty;
            }
        }

        if (_dynamicAddedTags.TryGetValue(tag, out var addedTag))
        {
            foreach (object o in addedTag)
            {
                if (source.Equals(o))
                {
                    return false;
                }
            }

            addedTag.Add(source);
        }
        else
        {
            var list = _pool.Get() as List<object>;
            list.Add(source);
            _dynamicAddedTags.Add(tag, list);
        }

        return true;
    }

    private bool TryAddDynamicRemovedTag<T>(T source, Tag tag)
    {
        if (!(source is Effect) && !(source is Ability)) return false;
        var dirty = false;
        if (_dynamicAddedTags.TryGetValue(tag, out var addedTag))
        {
            addedTag.Clear();
            _pool.Return(addedTag);
            dirty = _dynamicAddedTags.Remove(tag);
        }

        if (!IsTagInList(tag, _fixedTags)) return dirty;

        if (_dynamicRemovedTags.TryGetValue(tag, out var removedTag))
            removedTag.Add(source);
        else
        {
            var list = _pool.Get() as List<object>;
            list.Add(source);
            _dynamicRemovedTags.Add(tag, list);
        }

        return true;
    }

    private bool TryRemoveDynamicTag<T>(ref Dictionary<Tag, List<object>> dynamicTag, T source,
        Tag tag)
    {
        var dirty = false;

        if (source is Effect || source is Ability)
        {
            var hasValue = dynamicTag.TryGetValue(tag, out var tagList);
            if (hasValue)
            {
                tagList.Remove(source);
                dirty = tagList.Count == 0;
                if (dirty)
                {
                    _pool.Return(tagList);
                    dynamicTag.Remove(tag);
                }
            }
        }

        return dirty;
    }

    private bool TryRemoveDynamicAddedTag<T>(T source, Tag tag)
    {
        return TryRemoveDynamicTag(ref _dynamicAddedTags, source, tag);
    }

    private bool TryRemoveDynamicRemovedTag<T>(T source, Tag tag)
    {
        return TryRemoveDynamicTag(ref _dynamicRemovedTags, source, tag);
    }

    /// <summary>
    /// 应用Effect的动态标签
    /// </summary>
    /// <param name="source"></param>
    public void ApplyEffectDynamicTag(Effect source)
    {
        var tagIsDirty = false;
        var grantedTagSet = source.GrantedTags;
        foreach (var tag in grantedTagSet.Tags)
        {
            var dirty = TryAddDynamicAddedTag(source, tag);
            tagIsDirty = tagIsDirty || dirty;
        }

        if (tagIsDirty) TagIsDirty(grantedTagSet);
    }

    public void ApplyAbilityDynamicTag(Ability source)
    {
        var tagIsDirty = false;
        var activationOwnedTag = source.ActivationOwnedTags;
        foreach (var tag in activationOwnedTag.Tags)
        {
            var dirty = TryAddDynamicAddedTag(source, tag);
            tagIsDirty = tagIsDirty || dirty;
        }

        if (tagIsDirty) TagIsDirty(activationOwnedTag);
    }

    public void RestoreDynamicTags<T>(T source, TagSet tagSet)
    {
        var tagIsDirty = false;
        foreach (var tag in tagSet.Tags)
        {
            var dirty = TryRemoveDynamicAddedTag(source, tag);
            tagIsDirty = tagIsDirty || dirty;
        }

        if (tagIsDirty) TagIsDirty(tagSet);
    }

    public void RestoreEffectDynamicTags(Effect effect)
    {
        RestoreDynamicTags(effect, effect.GrantedTags);
    }

    public void RestoreAbilityDynamicTags(Ability ability)
    {
        RestoreDynamicTags(ability, ability.ActivationOwnedTags);
    }

    public bool HasTag(Tag tag)
    {
        // LINQ表达式存在GC，且HasTag调用频率很高，所以这里全都使用foreach
        var fixedTagsContainsTag = false;
        foreach (var t in _fixedTags)
        {
            if (t.HasTag(tag))
            {
                fixedTagsContainsTag = true;
                break;
            }
        }

        var dynamicAddedTagsContainsTag = false;
        foreach (var t in _dynamicAddedTags)
        {
            if (t.Key.HasTag(tag))
            {
                dynamicAddedTagsContainsTag = true;
                break;
            }
        }

        var dynamicRemovedTagsContainsTag = false;
        foreach (var t in _dynamicRemovedTags)
        {
            if (t.Key.HasTag(tag))
            {
                dynamicRemovedTagsContainsTag = true;
                break;
            }
        }

        return (fixedTagsContainsTag || dynamicAddedTagsContainsTag) && !dynamicRemovedTagsContainsTag;
    }

    public bool HasAllTags(TagSet other)
    {
        if (other.Empty) return true;
        foreach (var tag in other.Tags)
            if (!HasTag(tag))
                return false;

        return true;
    }

    public bool HasAllTags(params Tag[] tags)
    {
        foreach (var tag in tags)
            if (!HasTag(tag))
                return false;

        return true;
    }

    public bool HasAnyTags(TagSet other)
    {
        if (other.Empty) return false;
        foreach (var tag in other.Tags)
        {
            if (HasTag(tag)) return true;
        }

        return false;
        //return !other.Empty && other.Tags.Any(HasTag);
    }

    public bool HasAnyTags(params Tag[] tags)
    {
        bool hasAny = false;
        foreach (var tag in tags)
            if (HasTag(tag))
            {
                hasAny = true;
                break;
            }

        return hasAny;
        //return tags.Any(HasTag);
    }

    public bool HasNoneTags(TagSet other)
    {
        if (other.Empty) return true;
        foreach (var tag in other.Tags)
        {
            if (HasTag(tag)) return false;
        }

        return true;
        //return other.Empty || !other.Tags.Any(HasTag);
    }

    public bool HasNoneTags(params Tag[] tags)
    {
        if (tags.Length == 0) return true;
        foreach (var tag in tags)
        {
            if (HasTag(tag)) return false;
        }

        return true;
        //return !tags.Any(HasTag);
    }
}


