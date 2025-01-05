using System;
using System.Collections.Generic;

namespace Miros.Core;

public static class EffectExtensions
{
    public static void ApplyModWithInstant(this Effect effect)
    {
        if (effect.Modifiers == null) return;

        foreach (var modifier in effect.Modifiers)
            ApplyModifier(effect, modifier);
    }


    private static void ApplyModifier(Effect effect, Modifier modifier)
    {
        var attributeBase = effect.OwnerAgent.GetAttributeBase(modifier.AttributeSetTag, modifier.AttributeTag);

        if (attributeBase == null) return;

        // if (attribute.IsSupportOperation(modifier.Operation) == false)
        //     throw new InvalidOperationException("Unsupported operation.");

        if (attributeBase.CalculateMode != CalculateMode.Stacking)
            throw new InvalidOperationException(
                $"[EX] Instant GameplayEffect Can Only Modify Stacking Mode Attribute! " +
                $"But {modifier.AttributeSetTag}.{modifier.AttributeTag} is {attributeBase.CalculateMode}");

        var magnitude = modifier.CalculateMagnitude(effect);
        var baseValue = attributeBase.BaseValue;
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

        // Trigger Grabber

        attributeBase.SetBaseValue(baseValue);
        // AttributeSetContainer.Sets[modifier.AttributeSetTag]
        //     .ChangeAttributeBase(modifier.AttributeTag, baseValue);
    }



    public static void RemoveEffectWithAllTags(this Effect effect, TagSet tags)
    {
        if (tags.Empty || effect.Executor == null) return;

        if (effect.OwnedTags.HasAll(tags))
            effect.Executor.RemoveState(effect);

    }


    public static void RemoveEffectWithAnyTags(this Effect effect, TagSet tags)
    {
        if (tags.Empty || effect.Executor == null) return;

        if (effect.OwnedTags.HasAny(tags))
            effect.Executor.RemoveState(effect);
    }


}