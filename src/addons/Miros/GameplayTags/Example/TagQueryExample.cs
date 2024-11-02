public class GameplayTagQueryExample
{
    public void Example()
    {
        var tagManager = GameplayTagManager.Instance;
        
        // 创建一些标签
        var combatTag = tagManager.RequestGameplayTag("Combat");
        var meleeTag = tagManager.RequestGameplayTag("Combat.Melee");
        var swordTag = tagManager.RequestGameplayTag("Combat.Melee.Sword");
        var stunTag = tagManager.RequestGameplayTag("Status.Stun");
        
        // 创建一个标签容器
        var container = new GameplayTagContainer([]);
        container.AddTag(swordTag);
        
        // 基础查询
        bool hasSword = container.HasTag(swordTag); // 精确匹配
        bool hasMelee = container.HasTag(meleeTag, EGameplayTagMatchType.IncludeChildTags); // 包含子标签
        bool hasCombat = container.HasTag(combatTag, EGameplayTagMatchType.IncludeChildTags); // 包含子标签
        
        // 复杂查询
        var query = new GameplayTagQuery()
            .AddAllTag(meleeTag, EGameplayTagMatchType.IncludeChildTags) // 必须是近战武器
            .AddNoneTag(stunTag) // 不能被眩晕
            .AddAnyTag(swordTag); // 必须是剑类武器之一
            
        bool matchesQuery = container.MatchesQuery(query);
    }
}