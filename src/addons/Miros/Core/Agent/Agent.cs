using System;
using System.Collections.Generic;
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
    private readonly Dictionary<ExecutorType, IExecutor> _executors = [];
    private TagContainer _ownedTags;
    private AttributeSetContainer AttributeSetContainer { get; set; }


    public Node Host { get; private set; }
    public bool Enabled { get; private set; }
    public EventStream EventStream { get; private set; }


    public void Init(Node host)
    {
        Enabled = true;
        Host = host;
        EventStream = new EventStream();

        _ownedTags = new TagContainer([]);
        AttributeSetContainer = new AttributeSetContainer(this);

        // FIXME: 不应该在这里进行初始化
        _executors[ExecutorType.EffectExecutor] = new EffectExecutor();
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

    public void SwitchTaskFromTag(ExecutorType executorType, Tag tag, Context switchArgs = null)
    {
        var executor = GetExecutor(executorType);
        executor.SwitchStateByTag(tag, switchArgs);
    }


    public void AddActions(ExecutorType executorType, Context context, Type[] stateTypes)
    {
        foreach (var stateType in stateTypes)
            AddAction(executorType, context, stateType);
    }


    public void AddAction(ExecutorType executorType, Context context, Type stateType)
    {
        var state = (State)Activator.CreateInstance(stateType);

        state.Context = context;
        state.OwnerAgent = this;
        state.Task = TaskProvider.GetTask(state.TaskType);
        state.Init();

        if (executorType == ExecutorType.MultiLayerExecutor)
            PushStateOnExecutor(executorType, state);
    }


    public void AddEffect(Effect effect)
    {
        effect.OwnerAgent = this;
        effect.Task = TaskProvider.GetTask(effect.TaskType) as EffectTask;
        effect.Init();
        PushStateOnExecutor(ExecutorType.EffectExecutor, effect);
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

    private void PushStateOnExecutor(ExecutorType executorType, State state)
    {
        var executor = GetExecutor(executorType);
        executor.AddState(state);
    }


    public void RemoveState(ExecutorType executorType, State state)
    {
        if (!_executors.TryGetValue(executorType, out var executor))
        {
#if GODOT4 && DEBUG
            GD.Print($"[Miros] executor of {executorType} not found");
            return;
#endif
        }

        executor.RemoveState(state);
    }


    public void Update(double delta)
    {
        foreach (var executor in _executors.Values) executor.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var executor in _executors.Values) executor.PhysicsUpdate(delta);
    }

    // public void ApplyExecWithInstant(Effect effect)
    // {
    //     if (effect.Executions == null) return;

    //     foreach (var execution in effect.Executions)
    //     {
    //         execution.Execute(effect, out var modifierOptions);

    //         foreach (var modifierOption in modifierOptions)
    //         {
    //             var attribute = GetAttributeBase(modifierOption.AttributeName, modifierOption.AttributeSetName);
    //             var modifier = new Modifier(attribute.AttributeSetTag, attribute.AttributeTag,
    //                 modifierOption.Magnitude, modifierOption.Operation, modifierOption.MMC);

    //             ApplyModifier(effect, modifier, attribute);
    //         }
    //     }
    // }

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

        // Trigger Grabber

        AttributeSetContainer.Sets[modifier.AttributeSetTag]
            .ChangeAttributeBase(modifier.AttributeTag, baseValue);
    }


    public Effect[] GetRunningEffects()
    {
        return (_executors[ExecutorType.EffectExecutor] as EffectExecutor)
            .GetRunningEffects().ToArray();
    }


    /// <summary>
    ///     移除所有包含指定标签的Effect
    /// </summary>
    public void RemoveEffectWithAnyTags(TagSet tags)
    {
        if (tags.Empty) return;
        if (!_executors.TryGetValue(ExecutorType.EffectExecutor, out var executor)) return;
        var effects = (executor as EffectExecutor).GetRunningEffects();
        var removeList = new List<State>();

        foreach (var effect in effects)
        {
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