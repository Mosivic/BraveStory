using System;
using System.Collections.Generic;

namespace Miros.Core;

public interface IMiroSystemNode
    {
        void Init(GameplayTag[] baseTags, Type[] attrSetTypes, AbilityAsset[] baseAbilities,int level);
        
        void SetLevel(int level);
        
        bool HasTag(GameplayTag tag);
        
        bool HasAllTags(GameplayTagSet tags);
        
        bool HasAnyTags(GameplayTagSet tags);
        
        void AddFixedTags(GameplayTagSet tags);
        void AddFixedTag(GameplayTag gameplayTag);
        
        void RemoveFixedTags(GameplayTagSet tags);
        void RemoveFixedTag(GameplayTag gameplayTag);

        EffectState ApplyGameplayEffectTo(EffectState effect,IMiroSystemNode target);
        
        EffectState ApplyGameplayEffectToSelf(EffectState effect);

        void ApplyModFromInstantGameplayEffect(EffectState spec);
        
        void RemoveGameplayEffect(EffectState spec);
        
        void Tick();
        
        Dictionary<string,float> DataSnapshot();
        
        AbilitySpec GrantAbility(AbstractAbility ability);
        
        void RemoveAbility(string abilityName);
        
        float? GetAttributeCurrentValue(string setName,string attributeShortName);
        float? GetAttributeBaseValue(string setName,string attributeShortName);

        bool TryActivateAbility(string abilityName, params object[] args);
        void TryEndAbility(string abilityName);

        CooldownTimer CheckCooldownFromTags(GameplayTagSet tags);
        
        T AttrSet<T>() where T : AttributeSet;

        void ClearGameplayEffect();
    }
}