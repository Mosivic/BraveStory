public class EffectSystemExample
{
    private GameplayTagManager _tagManager;
    private GameplayEffectComponent _effectComponent;
    
    public void Initialize()
    {
        _tagManager = GameplayTagManager.Instance;
        
        // 创建一些效果
        var stunEffect = new GameplayEffect("Stun")
        {
            Duration = 3.0f
        };
        
        // 添加效果的标签要求
        stunEffect.RequiredTags.AddTag(_tagManager.RequestGameplayTag("Status.CanBeStunned"));
        stunEffect.IgnoredTags.AddTag(_tagManager.RequestGameplayTag("Status.StunImmune"));
        stunEffect.GrantedTags.AddTag(_tagManager.RequestGameplayTag("Status.Stunned"));
        
        // 创建一个燃烧效果
        var burnEffect = new GameplayEffect("Burn")
        {
            Duration = 5.0f
        };
        burnEffect.IgnoredTags.AddTag(_tagManager.RequestGameplayTag("Status.FireImmune"));
        burnEffect.GrantedTags.AddTag(_tagManager.RequestGameplayTag("Status.Burning"));
        
        // 应用效果
        _effectComponent.TryApplyEffect(stunEffect);
        _effectComponent.TryApplyEffect(burnEffect);
    }
}