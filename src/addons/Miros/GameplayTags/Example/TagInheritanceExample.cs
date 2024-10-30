using Godot;

public class TagInheritanceExample
{
    private GameplayTagManager _tagManager;
    private GameplayTagInheritance _inheritance;
    
    public void Initialize()
    {
        _tagManager = GameplayTagManager.Instance;
        
        // 创建一些标签
        var weaponTag = _tagManager.RequestGameplayTag("Item.Weapon");
        var swordTag = _tagManager.RequestGameplayTag("Item.Weapon.Sword");
        var magicSwordTag = _tagManager.RequestGameplayTag("Item.Weapon.Sword.Magic");
        
        // 设置基础武器属性
        _inheritance.SetProperty(weaponTag, "BaseDamage", 10);
        _inheritance.SetProperty(weaponTag, "DurabilityMax", 100);
        
        // 剑继承自武器，并添加自己的属性
        _inheritance.AddInheritance(swordTag, weaponTag);
        _inheritance.SetProperty(swordTag, "AttackSpeed", 1.2f);
        
        // 魔法剑继承自剑
        _inheritance.AddInheritance(magicSwordTag, swordTag);
        _inheritance.SetProperty(magicSwordTag, "MagicDamage", 5);
        
        // 使用继承的属性
        if (_inheritance.TryGetProperty<int>(magicSwordTag, "BaseDamage", out var damage))
        {
            GD.Print($"Magic Sword Base Damage: {damage}"); // 输出：10
        }
        
        if (_inheritance.TryGetProperty<float>(magicSwordTag, "AttackSpeed", out var speed))
        {
            GD.Print($"Magic Sword Attack Speed: {speed}"); // 输出：1.2
        }
    }
}