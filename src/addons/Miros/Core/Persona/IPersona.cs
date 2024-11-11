using System;
using System.Collections.Generic;

namespace Miros.Core;

public interface IPersona
{

    void Init(Tag[] baseTags, Type[] attrSetTypes, Ability[] baseAbilities, int level);

    bool HasTag(Tag tag);

    bool HasAllTags(TagSet tags);

    bool HasAnyTags(TagSet tags);

    void AddFixedTags(TagSet tags);

    void AddFixedTag(Tag tag);

    void RemoveFixedTags(TagSet tags);

    void RemoveFixedTag(Tag tag);
    
    void AddEffect(Effect effect);

    void ApplyEffectTo(Effect effect, IPersona target);

    void ApplyEffectToSelf(Effect effect);

    void ApplyModFromInstantEffect(Effect effect);

    void RemoveEffect(Effect effect);

    void Tick(double delta);

    Dictionary<string, float> DataSnapshot();

    void GrantAbility(Ability ability);
    
    void RemoveAbility(string abilityName);

    float? GetAttributeCurrentValue(string setName, string attributeShortName);

    float? GetAttributeBaseValue(string setName, string attributeShortName);

    bool TryActivateAbility(string abilityName, params object[] args);

    void TryEndAbility(string abilityName);

    CooldownTimer CheckCooldownFromTags(TagSet tags);

    T AttrSet<T>() where T : AttributeSet;

    void ClearEffect();

}   

