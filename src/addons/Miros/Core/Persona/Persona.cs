using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public abstract class Persona : AbsPersona, IPersona
{
    public int Level { get; protected set; }

    public bool Enabled { get; set; }
    public EffectContainer EffectContainer { get; private set; }

    public TagAggregator TagAggregator { get; private set; }

    public AbilityContainer AbilityContainer { get; private set; }

    public AttributeSetContainer AttributeSetContainer { get; private set; }

    private bool _ready;


    private void Prepare()
    {
        if (_ready) return;
        AbilityContainer = new AbilityContainer(this);
        EffectContainer = new EffectContainer(this);
        AttributeSetContainer = new AttributeSetContainer(this);
        TagAggregator = new TagAggregator(this);
        _ready = true;
    }

    public void Enable()
    {
        AttributeSetContainer.OnEnable();
    }

    public void Disable()
    {
        AttributeSetContainer.OnDisable();
        DisableAllAbilities();
        ClearEffect();
        TagAggregator?.OnDisable();
    }

    private void Awake()
    {
        Prepare();
    }

    private void OnEnable()
    {
        Prepare();
        TagAggregator?.OnEnable();
        Enable();
    }

    private void OnDisable()
    {
        Disable();
    }

    public void Init(Tag[] baseTags, Type[] attrSetTypes, Ability[] baseAbilities, int level)
    {
        Prepare();
        SetLevel(level);
        if (baseTags != null) TagAggregator.Init(baseTags);

        if (attrSetTypes != null)
        {
            foreach (var attrSetType in attrSetTypes)
                AttributeSetContainer.AddAttributeSet(attrSetType);
        }

        if (baseAbilities != null)
        {
            foreach (var ability in baseAbilities)
                AbilityContainer.GrantAbility(ability);
        }
    }



    public void SetLevel(int level)
    {
        Level = level;
    }

    public bool HasTag(Tag gameplayTag)
    {
        return TagAggregator.HasTag(gameplayTag);
    }

    public bool HasAllTags(TagSet tags)
    {
        return TagAggregator.HasAllTags(tags);
    }

    public bool HasAnyTags(TagSet tags)
    {
        return TagAggregator.HasAnyTags(tags);
    }

    public void AddFixedTags(TagSet tags)
    {
        TagAggregator.AddFixedTag(tags);
    }

    public void RemoveFixedTags(TagSet tags)
    {
        TagAggregator.RemoveFixedTag(tags);
    }

    public void AddFixedTag(Tag gameplayTag)
    {
        TagAggregator.AddFixedTag(gameplayTag);
    }

    public void RemoveFixedTag(Tag gameplayTag)
    {
        TagAggregator.RemoveFixedTag(gameplayTag);
    }

    public void AddEffect(Effect effect)
    {
        EffectContainer.AddEffect(this, effect);
    }

    public void RemoveEffect(Effect effect)
    {
        EffectContainer.RemoveEffect(effect);
    }

    public void RemoveEffectWithAnyTags(TagSet tags)
    {
        EffectContainer.RemoveEffectWithAnyTags(tags);
    }

    public void ApplyEffectTo(Effect effect, IPersona target)
    {
        target.AddEffect(effect);
    }


    public void ApplyEffectTo(Effect effect, IPersona target, int effectLevel)
    {
        effect.Level = effectLevel;
        target.AddEffect(effect);
    }


    public void ApplyEffectToSelf(Effect effect)
    {
        ApplyEffectTo(effect, this);
    }

    public void GrantAbility(Ability ability)
    {
        AbilityContainer.GrantAbility(ability);
    }

    public void RemoveAbility(string abilityName)
    {
        AbilityContainer.RemoveAbility(abilityName);
    }

    public AttributeValue? GetAttributeAttributeValue(string attrSetName, string attrShortName)
    {
        var value = AttributeSetContainer.GetAttributeAttributeValue(attrSetName, attrShortName);
        return value;
    }

    public CalculateMode? GetAttributeCalculateMode(string attrSetName, string attrShortName)
    {
        var value = AttributeSetContainer.GetAttributeCalculateMode(attrSetName, attrShortName);
        return value;
    }

    public float? GetAttributeCurrentValue(string setName, string attributeShortName)
    {
        var value = AttributeSetContainer.GetAttributeCurrentValue(setName, attributeShortName);
        return value;
    }

    public float? GetAttributeBaseValue(string setName, string attributeShortName)
    {
        var value = AttributeSetContainer.GetAttributeBaseValue(setName, attributeShortName);
        return value;
    }

    public void Tick()
    {
        AbilityContainer.Tick();
        EffectContainer.Tick();
    }

    public Dictionary<string, float> DataSnapshot()
    {
        return AttributeSetContainer.Snapshot();
    }

    public bool TryActivateAbility(string abilityName, params object[] args)
    {
        return AbilityContainer.TryActivateAbility(abilityName, args);
    }

    public void TryEndAbility(string abilityName)
    {
        AbilityContainer.EndAbility(abilityName);
    }

    public void TryCancelAbility(string abilityName)
    {
        AbilityContainer.CancelAbility(abilityName);
    }

    public void ApplyModFromInstantEffect(Effect effect)
    {
        foreach (var modifier in effect.Modifiers)
        {
            var attributeValue = GetAttributeAttributeValue(modifier.AttributeSetName, modifier.AttributeShortName);
            if (attributeValue == null) continue;
            if (attributeValue.Value.IsSupportOperation(modifier.Operation) == false)
            {
                throw new InvalidOperationException("Unsupported operation.");
            }

            if (attributeValue.Value.CalculateMode != CalculateMode.Stacking)
            {
                throw new InvalidOperationException(
                    $"[EX] Instant GameplayEffect Can Only Modify Stacking Mode Attribute! " +
                    $"But {modifier.AttributeSetName}.{modifier.AttributeShortName} is {attributeValue.Value.CalculateMode}");
            }

            var magnitude = modifier.CalculateMagnitude(effect, modifier.Magnitude);
            var baseValue = attributeValue.Value.BaseValue;
            switch (modifier.Operation)
            {
                case ModifierOperation.Add:
                    baseValue += magnitude;
                    break;
                case ModifierOperation.Minus:
                    baseValue -= magnitude;
                    break;
                case ModifierOperation.Multiply:
                    baseValue *= magnitude;
                    break;
                case ModifierOperation.Divide:
                    baseValue /= magnitude;
                    break;
                case ModifierOperation.Override:
                    baseValue = magnitude;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            AttributeSetContainer.Sets[modifier.AttributeSetName]
                .ChangeAttributeBase(modifier.AttributeShortName, baseValue);
        }
    }

    public CooldownTimer CheckCooldownFromTags(TagSet tags)
    {
        return EffectContainer.CheckCooldownFromTags(tags);
    }

    public T AttrSet<T>() where T : AttributeSet
    {
        AttributeSetContainer.TryGetAttributeSet<T>(out var attrSet);
        return attrSet;
    }

    public void ClearEffect()
    {
        // _abilityContainer = new AbilityContainer(this);
        // GameplayEffectContainer = new GameplayEffectContainer(this);
        // _attributeSetContainer = new AttributeSetContainer(this);
        // tagAggregator = new GameplayTagAggregator(this);
        EffectContainer.ClearEffect();
    }


    private void DisableAllAbilities()
    {
        AbilityContainer.CancelAllAbilities();
    }

}



