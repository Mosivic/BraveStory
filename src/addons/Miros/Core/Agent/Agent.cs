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



public partial class Agent : Node
{
	private readonly Dictionary<ExecutorType, IExecutor> _executors = [];
	private AttributeSetContainer AttributeSetContainer { get; set; }
	private TagContainer _ownedTags;
	private readonly StateExecutionRegistry _stateExecutionRegistry = new();
	private ITaskProvider _taskProvider;


	public Node2D Host { get; private set; }
	public bool Enabled { get;private set; }
	public EventStream EventStream { get; private set; }


	public void Initialize(Node2D host, Type[] attrSetTypes)
	{
		Enabled = true;
		Host = host;
		EventStream = new EventStream();
        
		_taskProvider = new StaticTaskProvider();
		_ownedTags = new TagContainer([]);
		AttributeSetContainer = new AttributeSetContainer(this);
		_executors[ExecutorType.EffectExecutor] = new EffectExecutor(this);;

		foreach (var attrSetType in attrSetTypes)
			AddAttributeSet(attrSetType);
	}


	public override void _Process(double delta)
	{
		if(Enabled) Update(delta);
	}


	public override void _PhysicsProcess(double delta)
	{
		if(Enabled) PhysicsUpdate(delta);
	}


	public float Attr(string attrName, AttributeValueType valueType = AttributeValueType.CurrentValue)
	{
		return AttributeSetContainer.Attribute(attrName, valueType);
	}


	public void CreateMultiLayerStateMachine(Tag layer, State defaultState, HashSet<State> states,
		StateTransitionConfig transitions)
	{
		var executor = new MultiLayerStateMachine();
		var container = new StateTransitionContainer();

		foreach (var state in states)
		{
			var task = _taskProvider.GetTask(state);
			state.Owner = this;
			_stateExecutionRegistry.AddStateExecutionContext(state.Tag,
				new StateExecutionContext(state, task, executor));
		}

		foreach (var transition in transitions.AnyTransitions)
			container.AddAny(new StateTransition(_stateExecutionRegistry.GetTask(transition.ToState),
				transition.Condition,
				transition.Mode));

		foreach (var (fromState, stateTransitions) in transitions.Transitions)
		foreach (var transition in stateTransitions)
			container.Add(_stateExecutionRegistry.GetTask(fromState),
				new StateTransition(_stateExecutionRegistry.GetTask(transition.ToState), transition.Condition,
					transition.Mode));


		executor.AddLayer(layer, _stateExecutionRegistry.GetTask(defaultState), container);
		_executors[ExecutorType.MultiLayerStateMachine] = executor;
	}


	public EffectExecutor GetEffectExecutor()
	{
		return _executors[ExecutorType.EffectExecutor] as EffectExecutor;
	}


	public void AddState(ExecutorType executorType, State state)
	{
		if (!_executors.TryGetValue(executorType, out var executor))
		{
#if GODOT4 && DEBUG
			throw new Exception($"[Miros.Connect] executor of {executorType} not found");
#else
			return;
#endif
		}

		state.Owner = this;
		var task = _taskProvider.GetTask(state);
		executor.AddTask(task);
		_stateExecutionRegistry.AddStateExecutionContext(state.Tag, new StateExecutionContext(state, task, executor));
	}


	public void AddStateTo(ExecutorType executorType, State state, Agent target)
	{
		target.AddState(executorType, state);
	}


	public void RemoveState(ExecutorType executorType, State state)
	{
		if (!_executors.TryGetValue(executorType, out var executor))
		{
#if GODOT4 && DEBUG
			throw new Exception($"[Miros.Connect] executor of {executorType} not found");
#else
			return;
#endif
		}

		var task = _stateExecutionRegistry.GetTask(state);
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

	public void ApplyExecWithInstant(Effect effect)
	{
		if (effect.Executions == null) return;

		foreach (var execution in effect.Executions)
		{
			execution.Execute(effect, out var modifierOptions);

			foreach (var modifierOption in modifierOptions)
			{
				var attribute = GetAttributeBase(modifierOption.AttributeName, modifierOption.AttributeSetName);
				var modifier = new Modifier(attribute.AttributeSetTag, attribute.AttributeTag,
					modifierOption.Magnitude, modifierOption.Operation, modifierOption.MMC);

				ApplyModifier(effect, modifier, attribute);
			}
		}
	}

	public void ApplyModWithInstant(Effect effect)
	{
		if (effect.Modifiers == null) return;

		foreach (var modifier in effect.Modifiers)
			ApplyModifier(effect, modifier);
	}


	private void ApplyModifier(Effect effect, Modifier modifier,AttributeBase attributeBase = null)
	{
		attributeBase ??= GetAttributeBase(modifier.AttributeSetTag, modifier.AttributeTag);
			
		if (attributeBase == null) return;

		// if (attribute.IsSupportOperation(modifier.Operation) == false)
		//     throw new InvalidOperationException("Unsupported operation.");

		if (attributeBase.CalculateMode != CalculateMode.Stacking)
			throw new InvalidOperationException(
				$"[EX] Instant GameplayEffect Can Only Modify Stacking Mode Attribute! " +
				$"But {modifier.AttributeSetTag}.{modifier.AttributeTag} is {attributeBase	.CalculateMode}");

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

		AttributeSetContainer.Sets[modifier.AttributeSetTag]
			.ChangeAttributeBase(modifier.AttributeTag, baseValue);
	}

	public bool AreTasksFromSameSource(TaskBase task1, TaskBase task2)
	{
		var state1 = _stateExecutionRegistry.GetState(task1);
		var state2 = _stateExecutionRegistry.GetState(task2);

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
		return (_executors[ExecutorType.EffectExecutor] as EffectExecutor)
			.GetRunningTasks()
			.Select(task => _stateExecutionRegistry.GetState(task) as Effect).ToArray();
	}


	/// <summary>
	///     移除所有包含指定标签的Effect
	/// </summary>
	public void RemoveEffectWithAnyTags(TagSet tags)
	{
		if (tags.Empty) return;
		if (!_executors.TryGetValue(ExecutorType.EffectExecutor, out var executor)) return;
		var tasks = executor.GetAllTasks();
		var removeList = new List<State>();

		foreach (var task in tasks)
		{
			var effectTask = task as EffectTask;
			var effect = _stateExecutionRegistry.GetState(effectTask) as Effect;

			var ownedTags = effect.OwnedTags;
			if (!ownedTags.Empty && ownedTags.HasAny(tags))
				removeList.Add(effect);

			var grantedTags = effect.GrantedTags;
			if (!grantedTags.Empty && grantedTags.HasAny(tags))
				removeList.Add(effect);
		}

		foreach (var effect in removeList) RemoveState(ExecutorType.EffectExecutor, effect);
	}

	
	#region Tag Check

	public bool HasTag(Tag gameplayTag)
	{
		return _ownedTags.HasTag(gameplayTag);
	}

	public bool HasAll(TagSet tags)
	{
		return _ownedTags.HasAll(tags);
	}

	public bool HasAny(TagSet tags)
	{
		return _ownedTags.HasAny(tags);
	}

	#endregion	



	#region AttributeSet

	public void AddAttributeSet(Type attrSetType)
	{
		AttributeSetContainer.AddAttributeSet(attrSetType);
	}

	public AttributeBase GetAttributeBase(Tag attrSetTag, Tag attrTag)
	{
		if (AttributeSetContainer.TryGetAttributeBase(attrSetTag, attrTag, out var value))
			return value;
		return null;
	}

	public AttributeBase GetAttributeBase(string attrName, string attrSetName = "")
	{
		if(AttributeSetContainer.TryGetAttributeBase(attrName, out var value, attrSetName))
			return value;
		return null;
	}


	public Dictionary<Tag, float> DataSnapshot()
	{
		return AttributeSetContainer.Snapshot();
	}

	#endregion



}
