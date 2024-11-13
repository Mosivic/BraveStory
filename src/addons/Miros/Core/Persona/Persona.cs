using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Godot;

namespace Miros.Core;

struct StateJobMap
{
    public State State;
    public JobBase Job;
    public SchedulerBase<JobBase> Scheduler;
}

public class Persona : AbsPersona, IPersona
{
    public string Name { get; set; }
    public int Level { get; protected set; }
    public bool Enabled { get; set; }

    public TagAggregator TagAggregator { get; private set; }
    public AttributeSetContainer AttributeSetContainer { get; private set; }

    private IJobProvider _jobProvider;
    private Dictionary<Type, SchedulerBase<JobBase>> _schedulers = [];
    private List<StateJobMap> _stateJobMaps = [];


    // public AbilityScheduler AbilityScheduler()
    // {
    //     if(_schedulers.TryGetValue(typeof(AbilityScheduler),out var scheduler))
    //     {
    //         return scheduler as AbilityScheduler;
    //     }
    //     return null;
    // }

    // public EffectScheduler GetEffectScheduler()
    // {
    //     if(_schedulers.TryGetValue(typeof(EffectJob),out var scheduler))
    //     {
    //         return scheduler as EffectScheduler;
    //     }
    //     return null;
    // }

    public Effect[] GetEffects()
    {
        return _stateJobMaps.Where(map => map.Job is EffectJob).Select(map => map.State as Effect).ToArray();
    }


    public void AddScheduler(SchedulerBase<JobBase> scheduler, HashSet<State> states)
    {
        // _schedulers[typeof(TState)] = scheduler;
        foreach (var state in states)
        {
            var job = _jobProvider.GetJob(state);
            scheduler.AddJob(job);
            _stateJobMaps.Add(new StateJobMap { State = state, Job = job, Scheduler = scheduler });
        }
    }

    public void AddState<TState>(TState state)
        where TState : State
    {
        var type = state.GetType();
        if (!_schedulers.TryGetValue(type, out var scheduler))
        {
#if GODOT4 &&DEBUG
            throw new Exception($"[Miros.Connect] scheduler of {type} not found");
#else
            return;
#endif
        }
        var job = _jobProvider.GetJob(state);
        scheduler.AddJob(job);
        _stateJobMaps.Add(new StateJobMap { State = state, Job = job, Scheduler = scheduler });
    }


    public void AddStateTo(State state, IPersona target)
    {
        target.AddState(state);
    }


    public void RemoveState<TState>(TState state)
        where TState : State
    {
        var type = state.GetType();
        if (!_schedulers.TryGetValue(type, out var scheduler))
        {
#if GODOT4 &&DEBUG
            throw new Exception($"[Miros.Connect] scheduler of {type} not found");
#else
            return;
#endif
        }
        var job = _stateJobMaps.FirstOrDefault(map => map.State == state).Job;
        scheduler.RemoveJob(job);
    }

    public void RemoveEffectWithAnyTags(TagSet tags)
    {
        if (tags.Empty) return;
        if (!_schedulers.TryGetValue(typeof(EffectJob), out var scheduler))
        {
            return;
        }
        var jobs = scheduler.GetAllJobs();
        var removeList = new List<State>();
        
        foreach (var job in jobs)
        {
            var effectJob = job as EffectJob;
            Effect effect = _stateJobMaps.FirstOrDefault(map => map.Job == effectJob).State as Effect;

            var ownedTags = effect.OwnedTags;
            if (!ownedTags.Empty && ownedTags.HasAnyTags(tags))
                removeList.Add(effect);

            var grantedTags = effect.GrantedTags;
            if (!grantedTags.Empty && grantedTags.HasAnyTags(tags))
                removeList.Add(effect);
        }

        foreach (var effect in removeList) RemoveState(effect);
    }


    public void Update(double delta)
    {
        foreach (var scheduler in _schedulers.Values)
        {
            scheduler.Update(delta);
        }
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var scheduler in _schedulers.Values)
        {
            scheduler.PhysicsUpdate(delta);
        }
    }

    public JobBase[] GetAllJobs()
    {
        return [.. _stateJobMaps.Select(map => map.Job)];
    }


    public JobBase GetNowJob(Type stateType, Tag layer)
    {
        if (!_schedulers.TryGetValue(stateType, out var scheduler))
        {
            return null;
        }
        return scheduler.GetNowJob(layer);
    }

    public JobBase GetLastJob(Type stateType, Tag layer)
    {
        if (!_schedulers.TryGetValue(stateType, out var scheduler))
        {
            return null;
        }
        return scheduler.GetLastJob(layer);
    }

    public double GetCurrentJobTime(Type stateType, Tag layer)
    {
        if (!_schedulers.TryGetValue(stateType, out var scheduler))
        {
            return 0;
        }
        return scheduler.GetCurrentJobTime(layer);
    }

    public void Enable()
    {
        AttributeSetContainer = new AttributeSetContainer(this);
        TagAggregator = new TagAggregator(this);
        AttributeSetContainer.OnEnable();
    }

    public void Disable()
    {
        AttributeSetContainer.OnDisable();
        TagAggregator?.OnDisable();
    }


    // public void Init(Tag[] baseTags, Type[] attrSetTypes, Ability[] baseAbilities, int level)
    // {
    //     Prepare();
    //     Level = level;
    //     if (baseTags != null) TagAggregator.Init(baseTags);

    //     if (attrSetTypes != null)
    //     {
    //         foreach (var attrSetType in attrSetTypes)
    //             AttributeSetContainer.AddAttributeSet(attrSetType);
    //     }

    //     if (baseAbilities != null)
    //     {
    //         foreach (var ability in baseAbilities)
    //             AbilityContainer.GrantAbility(ability);
    //     }
    // }


    #region Tag Check
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

    #endregion


    #region Attrubute Setget
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

    public Dictionary<string, float> DataSnapshot()
    {
        return AttributeSetContainer.Snapshot();
    }

    public T AttrSet<T>() where T : AttributeSet
    {
        AttributeSetContainer.TryGetAttributeSet<T>(out var attrSet);
        return attrSet;
    }

    #endregion


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
}



