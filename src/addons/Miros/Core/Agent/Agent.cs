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

public class Agent : AbsAgent, IAgent
{
    public Agent(Node2D host, ITaskProvider taskProvider, Type[] attrSetTypes) : base(host, taskProvider)
    {
        AttributeSetContainer = new AttributeSetContainer(this);
        Executors[ExecutorType.EffectExecutor] = new EffectExecutor(this);;

        foreach (var attrSetType in attrSetTypes)
            AddAttributeSet(attrSetType);
    }


    public void CreateMultiLayerStateMachine(Tag layer, State defaultState, HashSet<State> states,
        StateTransitionConfig transitions)
    {
        var executor = new MultiLayerStateMachine();
        var container = new StateTransitionContainer();

        foreach (var state in states)
        {
            var task = TaskProvider.GetTask(state);
            state.Owner = this;
            StateExecutionRegistry.AddStateExecutionContext(state.Tag,
                new StateExecutionContext(state, task, executor));
        }

        foreach (var transition in transitions.AnyTransitions)
            container.AddAny(new StateTransition(StateExecutionRegistry.GetTask(transition.ToState),
                transition.Condition,
                transition.Mode));

        foreach (var (fromState, stateTransitions) in transitions.Transitions)
        foreach (var transition in stateTransitions)
            container.Add(StateExecutionRegistry.GetTask(fromState),
                new StateTransition(StateExecutionRegistry.GetTask(transition.ToState), transition.Condition,
                    transition.Mode));


        executor.AddLayer(layer, StateExecutionRegistry.GetTask(defaultState), container);
        Executors[ExecutorType.MultiLayerStateMachine] = executor;
    }


    public EffectExecutor GetEffectExecutor()
    {
        return Executors[ExecutorType.EffectExecutor] as EffectExecutor;
    }


    public void AddState(ExecutorType executorType, State state)
    {
        if (!Executors.TryGetValue(executorType, out var executor))
        {
#if GODOT4 && DEBUG
            throw new Exception($"[Miros.Connect] executor of {executorType} not found");
#else
			return;
#endif
        }

        state.Owner = this;
        var task = TaskProvider.GetTask(state);
        executor.AddTask(task);
        StateExecutionRegistry.AddStateExecutionContext(state.Tag, new StateExecutionContext(state, task, executor));
    }


    public void AddStateTo(ExecutorType executorType, State state, Agent target)
    {
        target.AddState(executorType, state);
    }


    public void RemoveState(ExecutorType executorType, State state)
    {
        if (!Executors.TryGetValue(executorType, out var executor))
        {
#if GODOT4 && DEBUG
            throw new Exception($"[Miros.Connect] executor of {executorType} not found");
#else
			return;
#endif
        }

        var task = StateExecutionRegistry.GetTask(state);
        executor.RemoveTask(task);
    }


    public void Update(double delta)
    {
        foreach (var executor in Executors.Values) executor.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var executor in Executors.Values) executor.PhysicsUpdate(delta);
    }

    public void ApplyExecWithInstant(Effect effect)
    {
        if (effect.Executions == null) return;

        foreach (var execution in effect.Executions)
        {
            execution.Execute(effect, out var modifierOptions);

            foreach (var modifierOption in modifierOptions)
			{
				var attribute = GetAttributeBase(modifierOption.AttributeSetName, modifierOption.AttributeName);
                var modifier = new Modifier(attribute.AttributeSetTag, attribute.AttributeTag,
                    modifierOption.Magnitude, modifierOption.Operation);
					
                ApplyModifier(effect, modifier);
            }
        }
    }

    public void ApplyModWithInstant(Effect effect)
    {
        if (effect.Modifiers == null) return;

        foreach (var modifier in effect.Modifiers)
            ApplyModifier(effect, modifier);
    }

    private void ApplyModifier(Effect effect, Modifier modifier)
    {
        var attribute = GetAttributeBase(modifier.AttributeSetTag, modifier.AttributeTag);
        if (attribute == null) return;

        // if (attribute.IsSupportOperation(modifier.Operation) == false)
        //     throw new InvalidOperationException("Unsupported operation.");

        if (attribute.CalculateMode != CalculateMode.Stacking)
            throw new InvalidOperationException(
                $"[EX] Instant GameplayEffect Can Only Modify Stacking Mode Attribute! " +
                $"But {modifier.AttributeSetTag}.{modifier.AttributeTag} is {attribute.CalculateMode}");

        var magnitude = modifier.CalculateMagnitude(effect);
        var baseValue = attribute.BaseValue;
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

        AttributeSetContainer.Sets[modifier.AttributeSetTag]
            .ChangeAttributeBase(modifier.AttributeTag, baseValue);
    }

    public bool AreTasksFromSameSource(TaskBase task1, TaskBase task2)
    {
        var state1 = StateExecutionRegistry.GetState(task1);
        var state2 = StateExecutionRegistry.GetState(task2);

        if (state1 == null || state2 == null)
            return false;
        return state1.Source == state2.Source;
    }

    // public AbilityExecutor AbilityExecutor()
    // {
    //     if(Executors.TryGetValue(typeof(AbilityExecutor),out var executor))
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

    public Effect[] GetRunningEffects()
    {
        return (Executors[ExecutorType.EffectExecutor] as EffectExecutor)
            .GetRunningTasks()
            .Select(task => StateExecutionRegistry.GetState(task) as Effect).ToArray();
    }


    /// <summary>
    ///     移除所有包含指定标签的Effect
    /// </summary>
    public void RemoveEffectWithAnyTags(TagSet tags)
    {
        if (tags.Empty) return;
        if (!Executors.TryGetValue(ExecutorType.EffectExecutor, out var executor)) return;
        var tasks = executor.GetAllTasks();
        var removeList = new List<State>();

        foreach (var task in tasks)
        {
            var effectTask = task as EffectTask;
            var effect = StateExecutionRegistry.GetState(effectTask) as Effect;

            var ownedTags = effect.OwnedTags;
            if (!ownedTags.Empty && ownedTags.HasAny(tags))
                removeList.Add(effect);

            var grantedTags = effect.GrantedTags;
            if (!grantedTags.Empty && grantedTags.HasAny(tags))
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





}