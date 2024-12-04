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
	public Agent(Node2D host, ITaskProvider taskProvider) : base(host, taskProvider)
	{
		AttributeSetContainer = new AttributeSetContainer(this);
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
			StateExecutionRegistry.AddStateExecutionContext(state.Tag, new StateExecutionContext(state, task, executor));
		}

		foreach (var transition in transitions.AnyTransitions)
			container.AddAny(new StateTransition(StateExecutionRegistry.GetTask(transition.ToState), transition.Condition,
				transition.Mode));

		foreach (var (fromState, stateTransitions) in transitions.Transitions)
			foreach (var transition in stateTransitions)
				container.Add(StateExecutionRegistry.GetTask(fromState),
					new StateTransition(StateExecutionRegistry.GetTask(transition.ToState), transition.Condition, transition.Mode));


		executor.AddLayer(layer, StateExecutionRegistry.GetTask(defaultState), container);
		Executors[ExecutorType.MultiLayerStateMachine] = executor;
	}

	public void CreateEffectExecutor()
	{
		var executor = new EffectExecutor(this);
		Executors[ExecutorType.EffectExecutor] = executor;
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


	public void ApplyModFromInstantEffect(Effect effect)
	{
		foreach (var modifier in effect.Modifiers)
		{
			var attributeValue = GetAttributeValue(modifier.AttributeSetTag, modifier.AttributeTag);
			if (attributeValue == null) continue;
			
			if (attributeValue.Value.IsSupportOperation(modifier.Operation) == false)
				throw new InvalidOperationException("Unsupported operation.");

			if (attributeValue.Value.CalculateMode != CalculateMode.Stacking)
				throw new InvalidOperationException(
					$"[EX] Instant GameplayEffect Can Only Modify Stacking Mode Attribute! " +
					$"But {modifier.AttributeSetTag}.{modifier.AttributeTag} is {attributeValue.Value.CalculateMode}");

			var magnitude = modifier.CalculateMagnitude(effect);
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

			AttributeSetContainer.Sets[modifier.AttributeSetTag]
				.ChangeAttributeBase(modifier.AttributeTag, baseValue);
		}
	}

	public bool AreTasksFromSameSource(TaskBase task1, TaskBase task2)
	{
		var state1 = StateExecutionRegistry.GetState(task1);
		var state2 = StateExecutionRegistry.GetState(task2);

		if (state1 == null || state2 == null) 
			return false;
		else
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
	/// 移除所有包含指定标签的Effect
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


	#region Tag Check

	public bool HasTag(Tag gameplayTag)
	{
		return OwnedTags.HasTag(gameplayTag);
	}

	public bool HasAll(TagSet tags)
	{
		return OwnedTags.HasAll(tags);
	}

	public bool HasAny(TagSet tags)
	{
		return OwnedTags.HasAny(tags);
	}

	#endregion


	#region AttributeSet

	public void AddAttributeSet(Type attrSetType)
	{
		AttributeSetContainer.AddAttributeSet(attrSetType);
	}

	public AttributeValue? GetAttributeValue(Tag attrSetSign, Tag attrSign)
	{
		var value = AttributeSetContainer.GetAttributeValue(attrSetSign, attrSign);
		return value;
	}

	public AttributeBase GetAttributeBase(Tag attrSetSign, Tag attrSign)
	{
		var value = AttributeSetContainer.Sets[attrSetSign][attrSign];
		return value;
	}

	public CalculateMode? GetAttributeCalculateMode(Tag attrSetSign, Tag attrSign)
	{
		var value = AttributeSetContainer.GetAttributeCalculateMode(attrSetSign, attrSign);
		return value;
	}

	public float? GetAttributeCurrentValue(Tag attrSetSign, Tag attrSign)
	{
		var value = AttributeSetContainer.GetAttributeCurrentValue(attrSetSign, attrSign);
		return value;
	}

	public float? GetAttributeBaseValue(Tag attrSetSign, Tag attrSign)
	{
		var value = AttributeSetContainer.GetAttributeBaseValue(attrSetSign, attrSign);
		return value;
	}

	public Dictionary<Tag, float> DataSnapshot()
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
