public class GameplayTagFilterExample
{
    public void Example()
    {
        var tagManager = GameplayTagManager.Instance;
        
        // 创建一些测试标签
        var weaponTag = tagManager.RequestGameplayTag("Item.Weapon");
        var meleeTag = tagManager.RequestGameplayTag("Item.Weapon.Melee");
        var swordTag = tagManager.RequestGameplayTag("Item.Weapon.Melee.Sword");
        var brokenTag = tagManager.RequestGameplayTag("Status.Broken");
        var rareTag = tagManager.RequestGameplayTag("Quality.Rare");
        
        // 创建一个测试物品
        var itemTags = new GameplayTagContainer();
        itemTags.AddTag(swordTag);
        itemTags.AddTag(rareTag);
        
        // 创建过滤器
        var filter = new GameplayTagFilter();
        
        // 创建第一个过滤组：必须是近战武器且不能损坏
        var combatGroup = filter.CreateGroup(GameplayTagFilter.EFilterOperator.And);
        filter.AddCondition(combatGroup, meleeTag, 
            EGameplayTagMatchType.IncludeChildTags, 
            GameplayTagFilter.EFilterType.Include);
        filter.AddCondition(combatGroup, brokenTag, 
            EGameplayTagMatchType.Explicit, 
            GameplayTagFilter.EFilterType.Exclude);
        
        // 创建第二个过滤组：必须是稀有品质
        var qualityGroup = filter.CreateGroup(GameplayTagFilter.EFilterOperator.Or);
        filter.AddCondition(qualityGroup, rareTag, 
            EGameplayTagMatchType.Explicit, 
            GameplayTagFilter.EFilterType.Include);
        
        // 执行过滤
        bool matches = filter.Matches(itemTags); // 应该返回 true
        
        // 使用链式API创建更复杂的过滤器
        var complexFilter = new GameplayTagFilter()
            .CreateGroup(GameplayTagFilter.EFilterOperator.And)
            .AddCondition(weaponTag, EGameplayTagMatchType.IncludeChildTags, GameplayTagFilter.EFilterType.Include)
            .AddCondition(brokenTag, EGameplayTagMatchType.Explicit, GameplayTagFilter.EFilterType.Exclude);
    }
}