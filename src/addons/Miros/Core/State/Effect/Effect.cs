using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public abstract class Effect : AbsState
{
    public Dictionary<Tag, float> ValueMapWithTag { get; } = [];
    public Dictionary<string, float> ValueMapWithName { get; } = [];

    public event Action<Persona, Effect> OnImmunity;
    public event Action<int, int> OnStackCountChanged;

    public DurationPolicy DurationPolicy { get; private set; }
    public  double Duration { get; set; } 
    public double Period { get; set; }
    public float Level { get; set; } = 1;
    public bool IsApplied { get; set; }
    public bool IsActive { get; set; }
    public int StackCount { get;  set; } = 1;

    public ExecutionCalculation[] Executions { get; set; }
    public EffectPeriodTicker PeriodTicker { get; }
    public Effect PeriodExecution { get; private set; }
    public Modifier[] Modifiers { get; private set; }
    public GrantedAbilityFromEffect[] GrantedAbility { get; private set; }

    public Dictionary<string, float> SnapshotSourceAttributes { get;  set; }
    public Dictionary<string, float> SnapshotTargetAttributes { get;  set; }

    public Cue[] CueOnExecute { get; set; }
    public Cue[] CueOnRemove { get; set; }
    public Cue[] CueOnAdd { get; set; }
    public Cue[] CueOnActivate { get; set; }
    public Cue[] CueOnDeactivate { get; set; }
    public HashSet<CueDurational> CueDurational { get; set; }

    //Stacking
    public EffectStacking Stacking { get; private set; }

    // TODO: Expiration Effects 
    public readonly Effect[] PrematureExpirationEffect;
    public readonly Effect[] RoutineExpirationEffectClasses;


    // Necessary for Job
    public void RaiseOnStackCountChanged(int oldStackCount, int newStackCount)
    {
        OnStackCountChanged?.Invoke(oldStackCount, newStackCount);
    }











}

    




