using System;
using Godot;


namespace Miros.Core;

public class GrantedAbilityFromEffect
{

    public readonly GrantedAbilityActivationPolicy ActivationPolicy;
    public readonly GrantedAbilityDeactivationPolicy DeactivationPolicy;
    public readonly GrantedAbilityRemovePolicy RemovePolicy;

    public readonly Effect SourceEffect;
    public readonly Persona Owner;
    
    public readonly string AbilityName;
    public readonly int AbilityLevel;
    public readonly Ability Ability;

    public GrantedAbilityFromEffect(Ability ability,
        Effect sourceEffect)
    {
        Ability = ability;
        SourceEffect = sourceEffect;
        AbilityName = Ability.Name;
        Owner = SourceEffect.Owner;

        // if (Owner.AbilityContainer.HasAbility(AbilityName))
        // {
        //     GD.PrintErr($"GrantedAbilityFromEffect: {Owner.Name} already has ability {AbilityName}");
        // }

        // Owner.GrantAbility(Ability);
        // Ability.SetLevel(AbilityLevel);

        // // 是否添加时激活
        // if (ActivationPolicy == GrantedAbilityActivationPolicy.WhenAdded)
        // {
        //     Owner.TryActivateAbility(AbilityName);
        // }

        // switch (RemovePolicy)
        // {
        //     case GrantedAbilityRemovePolicy.WhenEnd:
        //         Ability.RegisterEndAbility(RemoveSelf);
        //         break;
        //     case GrantedAbilityRemovePolicy.WhenCancel:
        //         Ability.RegisterCancelAbility(RemoveSelf);
        //         break;
        //     case GrantedAbilityRemovePolicy.WhenCancelOrEnd:
        //         Ability.RegisterEndAbility(RemoveSelf);
        //         Ability.RegisterCancelAbility(RemoveSelf);
        //         break;
        // }
    }

    // private void RemoveSelf()
    // {
    //     Owner.RemoveAbility(AbilityName);
    // }
}
