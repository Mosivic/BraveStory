using System;
using System.Collections.Generic;

namespace Miros.Core;

public class EffectExecutor : ExecutorBase, IExecutor
{
    private readonly List<Effect> _runningEffects = [];

    public override void Update(double delta)
    {
        base.Update(delta);
        UpdateTasks();

        foreach (var effect in _runningEffects) effect.Task.Update(effect, delta);
    }


    public override void SwitchStateByTag(Tag tag, Context context = null)
    {
        if (_states.TryGetValue(tag, out var state))
            if (state != null)
            {
                state.Enter();
                _runningEffects.Add(state as Effect);
                _onRunningEffectTasksIsDirty?.Invoke(this, state as Effect);
            }
    }

    public override void AddState(State state)
    {
        var isAddTask = true;
        var effect = state as Effect;

        foreach (var existingEffect in _runningEffects)
            if (AreTaskCouldStackByAnotherTask(effect, existingEffect))
            {
                // 如果Tag相同且来自同一个Agent, 则不添加, 并跳过当前循环
                if (effect.Tag == existingEffect.Tag && AreFromSameSourceAgent(effect, existingEffect))
                {
                    existingEffect.Task.Stack(existingEffect, true);
                    isAddTask = false;
                    continue;
                }

                // 如果来自不同Agent, 则根据是否可以叠加来决定是否添加
                if (AreFromSameSourceAgent(effect, existingEffect))
                    existingEffect.Task.Stack(existingEffect, true);
                else
                    existingEffect.Task.Stack(existingEffect, false);
            }

        if (isAddTask) base.AddState(effect);
    }

    public override void RemoveState(State state)
    {
        base.RemoveState(state);

        var effect = state as Effect;
        if (effect.Status == RunningStatus.Running)
        {
            _runningEffects.Remove(effect);
            _onRunningEffectTasksIsDirty?.Invoke(this, effect);
        }
    }


    public List<Effect> GetRunningEffects()
    {
        return _runningEffects;
    }

    private void UpdateTasks()
    {
        // Enter
        foreach (var state in _states.Values)
        {
            var effect = state as Effect;
            if (effect.DurationPolicy == DurationPolicy.Instant)
            {
                effect.Task.Enter(effect);
                effect.Task.Exit(effect);
            }
            else
            {
                effect.Task.Enter(effect);
                _runningEffects.Add(effect);
                _onRunningEffectTasksIsDirty?.Invoke(this, effect);
            }
        }

        // Exit
        foreach (var state in _runningEffects)
        {
            if (!state.CanExit())
                continue;

            state.Exit();
            _runningEffects.Remove(state);
            _onRunningEffectTasksIsDirty?.Invoke(this, state);
        }
    }

    private static bool AreTaskCouldStackByAnotherTask(Effect task, Effect otherTask)
    {
        return task.Stacking != null && otherTask.Stacking != null &&
               task.Stacking?.GroupTag == otherTask.Stacking?.GroupTag;
    }

    private bool AreFromSameSourceAgent(Effect effect1, Effect effect2)
    {
        if (effect1 == null || effect2 == null)
            return false;
        return effect1.SourceAgent == effect2.SourceAgent;
    }


    #region Event

    private EventHandler<Effect> _onRunningEffectTasksIsDirty;

    public void RegisterOnRunningEffectTasksIsDirty(EventHandler<Effect> handler)
    {
        _onRunningEffectTasksIsDirty += handler;
    }

    public void UnregisterOnRunningEffectTasksIsDirty(EventHandler<Effect> handler)
    {
        _onRunningEffectTasksIsDirty -= handler;
    }

    #endregion
}