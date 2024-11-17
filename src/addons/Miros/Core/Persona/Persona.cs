using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Miros.Core;

public enum ExecutorType
{
    MultiLayerStateMachine,
    EffectExecutor,
    AbilityExecutor
}

internal struct StateMap
{
    public State State;
    public TaskBase Task;
    public ExecutorBase<TaskBase> Executor;
}

public class Persona : AbsPersona, IPersona
{
    private readonly Dictionary<ExecutorType, ExecutorBase<TaskBase>> _executors = [];
    private readonly Node2D _host;
    private readonly Dictionary<Tag, StateMap> _stateMaps = [];
    private readonly ITaskProvider _taskProvider;

    public Persona(Node2D host, ITaskProvider taskProvider)
    {
        _taskProvider = taskProvider;
        AttributeSetContainer = new AttributeSetContainer(this);
    }

    public TagAggregator TagAggregator { get; private set; }
    
    public AttributeSetContainer AttributeSetContainer { get; set; }

    public State GetStateBy(Tag sign)
    {
        return _stateMaps.TryGetValue(sign, out var stateMap) ? stateMap.State : null;
    }

    public void CreateMultiLayerStateMachine(Tag layer, State defaultState, HashSet<State> states,
        StateTransitionConfig transitions)
    {
        var executor = new MultiLayerStateMachine();
        var container = new StateTransitionContainer();

        foreach (var state in states)
        {
            var task = _taskProvider.GetTask(state);
            _stateMaps[state.Sign] = new StateMap { State = state, Task = task, Executor = executor };
        }

        foreach (var transition in transitions.AnyTransitions)
            container.AddAny(new StateTransition(_stateMaps[transition.ToState.Sign].Task, transition.Condition,
                transition.Mode));

        foreach (var (fromState, stateTransitions) in transitions.Transitions)
        foreach (var transition in stateTransitions)
            container.Add(_stateMaps[fromState.Sign].Task,
                new StateTransition(_stateMaps[transition.ToState.Sign].Task, transition.Condition, transition.Mode));
                

        executor.AddLayer(layer, _stateMaps[defaultState.Sign].Task, container);
        _executors[ExecutorType.MultiLayerStateMachine] = executor;
    }


    public void AddState(ExecutorType executorType, State state)
    {
        if (!_executors.TryGetValue(executorType, out var executor))
        {
#if GODOT4 &&DEBUG
            throw new Exception($"[Miros.Connect] executor of {executorType} not found");
#else
			return;
#endif
        }

        var task = _taskProvider.GetTask(state);
        executor.AddTask(task);
        _stateMaps[state.Sign] = new StateMap { State = state, Task = task, Executor = executor };
    }


    public void AddStateTo(ExecutorType executorType, State state, Persona target)
    {
        target.AddState(executorType, state);
    }


    public void RemoveState(ExecutorType executorType, State state)
    {
        if (!_executors.TryGetValue(executorType, out var executor))
        {
#if GODOT4 &&DEBUG
            throw new Exception($"[Miros.Connect] executor of {executorType} not found");
#else
			return;
#endif
        }

        var task = _stateMaps[state.Sign].Task;
        executor.RemoveTask(task);
    }


    public void Update(double delta)
    {
        foreach (var executor in _executors.Values) executor.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var executor in _executors.Values) executor.PhysicsUpdate(delta);
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


    // public AbilityExecutor AbilityExecutor()
    // {
    //     if(_executors.TryGetValue(typeof(AbilityExecutor),out var executor))
    //     {
    //         return executor as AbilityExecutor;
    //     }
    //     return null;
    // }

    // public EffectExecutor GetEffectExecutor()
    // {
    //     if(_executors.TryGetValue(typeof(EffectTask),out var executor))
    //     {
    //         return executor as EffectExecutor;
    //     }
    //     return null;
    // }

    public Effect[] GetEffects()
    {
        return _executors[ExecutorType.EffectExecutor].GetAllTasks()
            .Select(task => _stateMaps[task.Sign].State as Effect).ToArray();
    }


    public void RemoveEffectWithAnyTags(TagSet tags)
    {
        if (tags.Empty) return;
        if (!_executors.TryGetValue(ExecutorType.EffectExecutor, out var executor)) return;
        var tasks = executor.GetAllTasks();
        var removeList = new List<State>();

        foreach (var task in tasks)
        {
            var effectTask = task as EffectTask;
            var effect = _stateMaps[effectTask.Sign].State as Effect;

            var ownedTags = effect.OwnedTags;
            if (!ownedTags.Empty && ownedTags.HasAnyTags(tags))
                removeList.Add(effect);

            var grantedTags = effect.GrantedTags;
            if (!grantedTags.Empty && grantedTags.HasAnyTags(tags))
                removeList.Add(effect);
        }

        foreach (var effect in removeList) RemoveState(ExecutorType.EffectExecutor, effect);
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