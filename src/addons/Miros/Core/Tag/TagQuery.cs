using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;
public class TagQuery
{
    private HashSet<GameplayTagRequirement> _anyRequirements = new();
    private HashSet<GameplayTagRequirement> _allRequirements = new();
    private HashSet<GameplayTagRequirement> _noneRequirements = new();


    public TagQuery(HashSet<Tag> allRequirements = null, HashSet<Tag> anyRequirements = null, HashSet<Tag> noneRequirements = null)
    {
        allRequirements?.ToList().ForEach(r => AddAllTag(r));
        anyRequirements?.ToList().ForEach(r => AddAnyTag(r));
        noneRequirements?.ToList().ForEach(r => AddNoneTag(r));
    }

    public TagQuery AddAnyTag(Tag tag, TagMatchType matchType = TagMatchType.Explicit)
    {
        _anyRequirements.Add(new GameplayTagRequirement(tag, matchType));
        return this;
    }

    public TagQuery AddAllTag(Tag tag, TagMatchType matchType = TagMatchType.Explicit)
    {
        _allRequirements.Add(new GameplayTagRequirement(tag, matchType));
        return this;
    }

    public TagQuery AddNoneTag(Tag tag, TagMatchType matchType = TagMatchType.Explicit)
    {
        _noneRequirements.Add(new GameplayTagRequirement(tag, matchType));
        return this;
    }

    public bool Matches(TagContainer container)
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
    public readonly Tag Tag;
    public readonly TagMatchType MatchType;

    public GameplayTagRequirement(Tag tag, TagMatchType matchType)
    {
        Tag = tag;
        MatchType = matchType;
    }
}