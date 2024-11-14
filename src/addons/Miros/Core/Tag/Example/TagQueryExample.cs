using Miros.Core;

public class GameplayTagQueryExample
{
    public void Example()
    {
        var tagManager = TagManager.Instance;

        // 创建一些标签
        var combatTag = tagManager.RequestTag("Combat");
        var meleeTag = tagManager.RequestTag("Combat.Melee");
        var swordTag = tagManager.RequestTag("Combat.Melee.Sword");
        var stunTag = tagManager.RequestTag("Status.Stun");

        // 创建一个标签容器
        var container = new TagContainer([]);
        container.AddTag(swordTag);

        // 基础查询
        var hasSword = container.HasTag(swordTag); // 精确匹配
        var hasMelee = container.HasTag(meleeTag, TagMatchType.IncludeChildTags); // 包含子标签
        var hasCombat = container.HasTag(combatTag, TagMatchType.IncludeChildTags); // 包含子标签

        // 复杂查询
        var query = new TagQuery()
            .AddAllTag(meleeTag, TagMatchType.IncludeChildTags) // 必须是近战武器
            .AddNoneTag(stunTag) // 不能被眩晕
            .AddAnyTag(swordTag); // 必须是剑类武器之一

        var matchesQuery = container.MatchesQuery(query);
    }
}