using System;
using System.Collections.Generic;

namespace Miros.Core;

public class EffectState : AbsState
{
    public DurationPolicy DurationPolicy { get; private set; }
    public float Duration { get; private set; }
    public float Period { get; private set; }
    public float ActivationTime {get;private set;}
    public float Level {get;private set;}
    public bool IsApplied {get;private set;}
    public bool IsActive {get;private set;}



    public List<ExecutionCalculation> Executions { get; private set; }
    public List<GrantedAbilityFromEffect> GrantedAbilities { get; private set; }
    public EffectTagContainer TagContainer { get; private set; }
    public float StackCount { get; internal set; }

    public readonly CueState[] CueOnExecute;
    public readonly CueState[] CueOnRemove;
    public readonly CueState[] CueOnAdd;
    public readonly CueState[] CueOnActivate;
    public readonly CueState[] CueOnDeactivate;
    public readonly CueState[] CueDurational;

    // Modifiers
    public readonly Modifier[] Modifiers;
    public readonly ExecutionCalculation[] Executions; // TODO: this should be a list of execution calculations

    // Granted Ability
    public readonly GrantedAbilityFromEffect[] GrantedAbilities;

    //Stacking
    public readonly GameplayEffectStacking Stacking;

    // TODO: Expiration Effects 
    public readonly EffectState[] PrematureExpirationEffect;
    public readonly EffectState[] RoutineExpirationEffectClasses;




}
