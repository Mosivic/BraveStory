using Miros.Core;

// 检查技能或效果分类关系的示例
public class AbilityTagExample
{
    private TagContainer _abilityTags;
    private TagManager _tagManager;

    public void Initialize()
    {
        _tagManager = TagManager.Instance;
        _abilityTags = new TagContainer([]);

        // 注册技能相关的标签
        var abilityTag = _tagManager.RequestTag("Ability");
        var attackTag = _tagManager.RequestTag("Ability.Attack");
        var meleeTag = _tagManager.RequestTag("Ability.Attack.Melee");
        var rangedTag = _tagManager.RequestTag("Ability.Attack.Ranged");
        var magicTag = _tagManager.RequestTag("Ability.Magic");
        var fireTag = _tagManager.RequestTag("Ability.Magic.Fire");
        var iceTag = _tagManager.RequestTag("Ability.Magic.Ice");

        // 添加技能标签
        _abilityTags.AddTag(meleeTag); // 这是一个近战技能
        _abilityTags.AddTag(fireTag); // 同时也是一个火焰魔法
    }

    public bool IsAttackAbility()
    {
        // 检查是否是攻击类技能（包括近战和远程）
        var attackTag = _tagManager.RequestTag("Ability.Attack");
        return _abilityTags.HasTag(attackTag, TagMatchType.IncludeChildTags);
    }

    public bool IsMeleeAttack()
    {
        // 检查是否是近战攻击
        var meleeTag = _tagManager.RequestTag("Ability.Attack.Melee");
        return _abilityTags.HasTag(meleeTag);
    }

    public bool IsMagicAbility()
    {
        // 检查是否是魔法技能
        var magicTag = _tagManager.RequestTag("Ability.Magic");
        return _abilityTags.HasTag(magicTag, TagMatchType.IncludeChildTags);
    }

    public bool IsFireMagic()
    {
        // 检查是否是火系魔法
        var fireTag = _tagManager.RequestTag("Ability.Magic.Fire");
        return _abilityTags.HasTag(fireTag);
    }

    public bool IsMeleeOrMagic()
    {
        // 使用复杂查询：检查是否是近战或魔法技能
        var query = new TagQuery()
            .AddAnyTag(_tagManager.RequestTag("Ability.Attack.Melee"))
            .AddAnyTag(_tagManager.RequestTag("Ability.Magic"),
                TagMatchType.IncludeChildTags);

        return _abilityTags.MatchesQuery(query);
    }
}