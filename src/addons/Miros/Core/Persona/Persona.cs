using System;
using System.Collections.Generic;
using System.Linq;
using BraveStory;
using Godot;

namespace Miros.Core;


public enum SchedulerType
{
    MultiLayerStateMachine,
    EffectScheduler,
    AbilityScheduler,
}

internal struct StateMap
{
    public State State;
    public JobBase Job;
    public SchedulerBase<JobBase> Scheduler;
}

public class Persona : AbsPersona, IPersona
{
    private readonly Node2D _host;
    private readonly IJobProvider _jobProvider;

    private readonly Dictionary<SchedulerType, SchedulerBase<JobBase>> _schedulers = [];
    private readonly Dictionary<Tag, StateMap> _stateMaps = [];



    public TagAggregator TagAggregator { get; private set; }
    public AttributeSetContainer AttributeSetContainer { get; set; } 

    public Persona(Node2D host, IJobProvider jobProvider)
    {
        _jobProvider = jobProvider;
        AttributeSetContainer = new AttributeSetContainer(this);
    }

    public State GetStateBy(Tag sign)
    {
        return _stateMaps.TryGetValue(sign, out var stateMap) ? stateMap.State : null;
    }

    public MultiLayerStateMachine CreateMultiLayerStateMachine(Tag layer,State defaultState, HashSet<State> states)
    {
        var scheduler = new MultiLayerStateMachine();
        var transitionsCache = new Dictionary<JobBase, HashSet<Transition>>();

        // 生成所有状态的job
        foreach (var state in states)
        {
            var job = _jobProvider.GetJob(state);
            _stateMaps[state.Sign] = new StateMap { State = state, Job = job, Scheduler = scheduler };
            transitionsCache[job] = state.Transitions;
        }

        var transitionRules = new Dictionary<JobBase, HashSet<StateTransition>>();
        var anyTransitionRules = new HashSet<StateTransition>();

        foreach (var (job, transitions) in transitionsCache)
        {
            foreach (var transition in transitions)
            {
                var toStateSign = transition.ToStateSign;
                if(toStateSign == Tags.None){
                    anyTransitionRules.Add(new StateTransition(
                        job,
                        transition.Condition,
                        transition.Mode
                    ));
                }else
                {
                    if(!transitionRules.ContainsKey(job))
                        transitionRules[job] = new HashSet<StateTransition>();

                    transitionRules[job].Add(new StateTransition(
                        _stateMaps[toStateSign].Job,
                        transition.Condition,
                        transition.Mode
                    ));
                }
            }
        }
        scheduler.AddLayer(layer,_stateMaps[defaultState.Sign].Job,transitionRules,anyTransitionRules);
        _schedulers[SchedulerType.MultiLayerStateMachine] = scheduler;

        return scheduler;
    }

    public void AddState(SchedulerType schedulerType, State state)
    {
        if (!_schedulers.TryGetValue(schedulerType, out var scheduler))
        {
#if GODOT4 &&DEBUG
            throw new Exception($"[Miros.Connect] scheduler of {schedulerType} not found");
#else
			return;
#endif
        }

        var job = _jobProvider.GetJob(state);
        scheduler.AddJob(job);
        _stateMaps[state.Sign] = new StateMap { State = state, Job = job, Scheduler = scheduler };
    }


    public void AddStateTo(SchedulerType schedulerType, State state, Persona target)
    {
        target.AddState(schedulerType, state);
    }


    public void RemoveState(SchedulerType schedulerType, State state)
    {
        if (!_schedulers.TryGetValue(schedulerType, out var scheduler))
        {
#if GODOT4 &&DEBUG
            throw new Exception($"[Miros.Connect] scheduler of {schedulerType} not found");
#else
			return;
#endif
        }

        var job = _stateMaps[state.Sign].Job;
        scheduler.RemoveJob(job);
    }


    public void Update(double delta)
    {
        foreach (var scheduler in _schedulers.Values) scheduler.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var scheduler in _schedulers.Values) scheduler.PhysicsUpdate(delta);
    }



    public void ApplyModFromInstantEffect(Effect effect)
    {
        foreach (var modifier in effect.Modifiers)
        {
            var attributeValue = GetAttributeAttributeValue(modifier.AttributeSetName, modifier.AttributeShortName);
            if (attributeValue == null) continue;
            if (attributeValue.Value.IsSupportOperation(modifier.Operation) == false)
                throw new InvalidOperationException("Unsupported operation.");

            if (attributeValue.Value.CalculateMode != CalculateMode.Stacking)
                throw new InvalidOperationException(
                    $"[EX] Instant GameplayEffect Can Only Modify Stacking Mode Attribute! " +
                    $"But {modifier.AttributeSetName}.{modifier.AttributeShortName} is {attributeValue.Value.CalculateMode}");

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
        return _schedulers[SchedulerType.EffectScheduler].GetAllJobs().Select(job => _stateMaps[job.Sign].State as Effect).ToArray();
    }


    public void RemoveEffectWithAnyTags(TagSet tags)
    {
        if (tags.Empty) return;
        if (!_schedulers.TryGetValue(SchedulerType.EffectScheduler, out var scheduler)) return;
        var jobs = scheduler.GetAllJobs();
        var removeList = new List<State>();

        foreach (var job in jobs)
        {
            var effectJob = job as EffectJob;
            var effect = _stateMaps[effectJob.Sign].State as Effect;

            var ownedTags = effect.OwnedTags;
            if (!ownedTags.Empty && ownedTags.HasAnyTags(tags))
                removeList.Add(effect);

            var grantedTags = effect.GrantedTags;
            if (!grantedTags.Empty && grantedTags.HasAnyTags(tags))
                removeList.Add(effect);
        }

        foreach (var effect in removeList) RemoveState(SchedulerType.EffectScheduler, effect);
    }



    // public void Enable()
    // {
    //     AttributeSetContainer = new AttributeSetContainer(this);
    //     TagAggregator = new TagAggregator(this);
    //     AttributeSetContainer.OnEnable();
    // }

    // public void Disable()
    // {
    //     AttributeSetContainer.OnDisable();
    //     // TagAggregator?.OnDisable();
    // }


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
}