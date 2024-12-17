using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Miros.Core;

public enum ExecutorType
{
    MultiLayerExecutor,
    EffectExecutor,
    NativeExecutor
}

public class Agent
{
    private readonly Dictionary<ExecutorType, IExecutor<State>> _executors = [];
    private TagContainer _ownedTags;
    private AttributeSetContainer AttributeSetContainer { get; set; }


    public Node Host { get; private set; }
    public bool Enabled { get; private set; }
    public EventStream EventStream { get; private set; }

    private EffectExecutor _effectExecutor;


    public void Init(Node host)
    {
        Enabled = true;
        Host = host;
        EventStream = new EventStream();

        _ownedTags = new TagContainer([]);
        AttributeSetContainer = new AttributeSetContainer(this);

		// FIXME: 不应该在这里进行初始化
        
        _effectExecutor = new EffectExecutor();
        ;
    }


    public void Process(double delta)
    {
        if (Enabled) Update(delta);
    }


    public void PhysicsProcess(double delta)
    {
        if (Enabled) PhysicsUpdate(delta);
    }


    public float Atr(string attrName, AttributeValueType valueType = AttributeValueType.CurrentValue)
    {
        return AttributeSetContainer.Attribute(attrName, valueType);
    }


    public EffectExecutor GetEffectExecutor()
    {
        return _executors[ExecutorType.EffectExecutor] as EffectExecutor;
    }

    public void SwitchTaskFromTag(ExecutorType executorType, Tag tag, Context context = null)
    {
        var executor = GetExecutor(executorType);
        executor.SwitchTaskByTag(tag, context);
    }


    public void AddTasksFromType<TState, THost, TContext, TExecuteArgs>(ExecutorType executorType,TContext context, Type[] tasks)
        where TState : State, new()
        where THost : Node
        where TContext : Context
        where TExecuteArgs : ExecuteArgs
    {
		foreach (var taskType in tasks)
			AddTaskFromType<TState, THost, TContext, TExecuteArgs>(executorType, context, taskType);
    }

	
	public void AddTaskFromType<TState, THost, TContext, TExecuteArgs>(ExecutorType executorType,TContext context, Type taskType)
	where TState : State, new()
	where THost : Node
	where TContext : Context
	where TExecuteArgs : ExecuteArgs
	{
		var task = (Task<TState, THost, TContext, TExecuteArgs>)Activator.CreateInstance(taskType);
		task.Init(this, Host as THost, context);

		if (executorType == ExecutorType.MultiLayerExecutor)
		{
			PushTaskOnExecutor(executorType, task, task.ExecuteArgs);
			task.State.OwnerTask = task;
			task.State.ExecutorType = executorType;
		}
		else if (executorType == ExecutorType.EffectExecutor)
		{
			PushTaskOnExecutor(executorType, task);
			task.State.OwnerTask = task;
			task.State.ExecutorType = executorType;
		}
	}
    
    
	public void AddTasksFromState(ExecutorType executorType, State[] states)
	{
		foreach (var state in states)
			AddTaskFromState(executorType, state);
	}


    public void AddTaskFromState(ExecutorType executorType, State state, ExecuteArgs args = null)
    {
        state.OwnerAgent = this;

        var task = TaskCreator.GetTask(state);
        PushTaskOnExecutor(executorType, task);

        task.State.OwnerTask = task;
        task.State.ExecutorType = executorType;
    }
    
    private IExecutor GetExecutor(ExecutorType executorType)
    {
        if (!_executors.TryGetValue(executorType, out var executor))
        {
            executor = executorType switch
            {
                ExecutorType.MultiLayerExecutor => new MultiLayerExecutor(),
                ExecutorType.EffectExecutor => new EffectExecutor(),
                _ => throw new ArgumentOutOfRangeException(nameof(executorType), executorType, null)
            };
            _executors[executorType] = executor;
        }
        return executor;
    }

    private void PushTaskOnExecutor(ExecutorType executorType, TaskBase task, ExecuteArgs args = null)
    {
        var executor = GetExecutor(executorType);
        executor.AddTask(task, args);
    }

    
    public void RemoveTaskByState(ExecutorType executorType, State state)
    {
        if (!_executors.TryGetValue(executorType, out var executor))
        {
#if GODOT4 && DEBUG
            GD.Print($"[Miros] executor of {executorType} not found");
            return;
#endif
        }

        executor.RemoveTask(state.OwnerTask);
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


    private void ApplyModifier(Effect effect, Modifier modifier, AttributeBase attributeBase = null)
    {
        attributeBase ??= GetAttributeBase(modifier.AttributeSetTag, modifier.AttributeTag);

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

        AttributeSetContainer.Sets[modifier.AttributeSetTag]
            .ChangeAttributeBase(modifier.AttributeTag, baseValue);
    }


    public Effect[] GetRunningEffects()
    {
        return (_executors[ExecutorType.EffectExecutor] as EffectExecutor)
            .GetRunningTasks()
            .Select(task => task.State as Effect).ToArray();
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
            var effect = effectTask.State as Effect;

            var ownedTags = effect.OwnedTags;
            if (!ownedTags.Empty && ownedTags.HasAny(tags))
                removeList.Add(effect);

            var grantedTags = effect.GrantedTags;
            if (!grantedTags.Empty && grantedTags.HasAny(tags))
                removeList.Add(effect);
        }

        foreach (var effect in removeList) RemoveTaskByState(ExecutorType.EffectExecutor, effect);
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
        if (AttributeSetContainer.TryGetAttributeBase(attrName, out var value, attrSetName))
            return value;
        return null;
    }


    public Dictionary<Tag, float> DataSnapshot()
    {
        return AttributeSetContainer.Snapshot();
    }

    #endregion
}