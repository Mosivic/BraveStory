using System;
using System.Linq;

namespace Miros.Core;

public readonly struct TagSet
{
    public readonly Tag[] Tags;

    public bool Empty => Tags == null || Tags.Length == 0;

    public TagSet(string[] tagNames)
    {
        Tags = new Tag[tagNames.Length];
        for (var i = 0; i < tagNames.Length; i++) Tags[i] = new Tag(tagNames[i]);
    }

    public TagSet(params Tag[] tags)
    {
        Tags = tags ?? [];
    }

    public bool Has(Tag tag)
    {
        return Tags.Any(t => t.HasTag(tag));
    }

    public bool HasAll(TagSet other)
    {
        return HasAll(other.Tags);
    }

    public bool HasAll(params Tag[] tags)
    {
        var set = this;
        return tags.All(tag => set.Has(tag));
    }

    public bool HasAny(TagSet other)
    {
        return HasAny(other.Tags);
    }

    public bool HasAny(params Tag[] tags)
    {
        var set = this;
        return tags.Any(tag => set.Has(tag));
    }

    public bool HasNone(TagSet other)
    {
        return HasNone(other.Tags);
    }

    public bool HasNone(params Tag[] tags)
    {
        var set = this;
        return tags.All(tag => !set.Has(tag));
    }
}