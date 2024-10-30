// 检查技能或效果分类关系的示例
public class AbilityTagExample
{
    private GameplayTagManager _tagManager;
    private GameplayTagContainer _abilityTags;
    
    public void Initialize()
    {
        _tagManager = GameplayTagManager.Instance;
        _abilityTags = new GameplayTagContainer();
        
        // 注册技能相关的标签
        var abilityTag = _tagManager.RequestGameplayTag("Ability");
        var attackTag = _tagManager.RequestGameplayTag("Ability.Attack");
        var meleeTag = _tagManager.RequestGameplayTag("Ability.Attack.Melee");
        var rangedTag = _tagManager.RequestGameplayTag("Ability.Attack.Ranged");
        var magicTag = _tagManager.RequestGameplayTag("Ability.Magic");
        var fireTag = _tagManager.RequestGameplayTag("Ability.Magic.Fire");
        var iceTag = _tagManager.RequestGameplayTag("Ability.Magic.Ice");
        
        // 添加技能标签
        _abilityTags.AddTag(meleeTag);  // 这是一个近战技能
        _abilityTags.AddTag(fireTag);   // 同时也是一个火焰魔法
    }
    
    public bool IsAttackAbility()
    {
        // 检查是否是攻击类技能（包括近战和远程）
        var attackTag = _tagManager.RequestGameplayTag("Ability.Attack");
        return _abilityTags.HasTag(attackTag, EGameplayTagMatchType.IncludeChildTags);
    }
    
    public bool IsMeleeAttack()
    {
        // 检查是否是近战攻击
        var meleeTag = _tagManager.RequestGameplayTag("Ability.Attack.Melee");
        return _abilityTags.HasTag(meleeTag);
    }
    
    public bool IsMagicAbility()
    {
        // 检查是否是魔法技能
        var magicTag = _tagManager.RequestGameplayTag("Ability.Magic");
        return _abilityTags.HasTag(magicTag, EGameplayTagMatchType.IncludeChildTags);
    }
    
    public bool IsFireMagic()
    {
        // 检查是否是火系魔法
        var fireTag = _tagManager.RequestGameplayTag("Ability.Magic.Fire");
        return _abilityTags.HasTag(fireTag);
    }
    
    public bool IsMeleeOrMagic()
    {
        // 使用复杂查询：检查是否是近战或魔法技能
        var query = new GameplayTagQuery()
            .AddAnyTag(_tagManager.RequestGameplayTag("Ability.Attack.Melee"))
            .AddAnyTag(_tagManager.RequestGameplayTag("Ability.Magic"), 
                EGameplayTagMatchType.IncludeChildTags);
                
        return _abilityTags.MatchesQuery(query);
    }
}