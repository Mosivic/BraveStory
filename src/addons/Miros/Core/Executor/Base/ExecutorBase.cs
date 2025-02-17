namespace Miros.Core;

public class ExecutorBase : AbsExecutor, IExecutor
{
    public virtual void AddState(State state)
    {
        var t = state;

        if (t.RemovePolicy != RemovePolicy.None)
            _tempStates.Add(t.Tag, t);

        _states.Add(t.Tag, t);
        t.Task.TriggerOnAdd(t);
    }

    public virtual void RemoveState(State state)
    {
        var t = state;

        if (t.RemovePolicy != RemovePolicy.None) // 如果任务有移除策略(非None)，则将任务从临时任务列表中移除
            _tempStates.Remove(t.Tag);

        if (t.Status == RunningStatus.Running) // 如果任务正在运行，则调用Exit方法
            t.Task.Exit(t);

        _states.Remove(t.Tag);
        t.Task.TriggerOnRemove(t);
    }


    public virtual void SwitchStateByTag(Tag tag, Context switchArgs)
    {
    }

    public virtual State GetLastState(Tag layer)
    {
        return null;
    }

    public virtual State GetNowState(Tag layer)
    {
        return null;
    }

    public virtual bool HasStateRunning(State state)
    {
        return false;
    }

    public virtual void PhysicsUpdate(double delta)
    {
    }


    public virtual void Update(double delta)
    {
        UpdateTempStates();
    }

    public virtual State[] GetAllStates()
    {
        return [.. _states.Values];
    }

    public virtual double GetCurrentStateTime(Tag layer)
    {
        return 0;
    }

    private void UpdateTempStates()
    {
        foreach (var state in _tempStates.Values) RemoveTempState(state);
    }

    private void RemoveTempState(State state)
    {
        var removePolicy = state.RemovePolicy;
        switch (removePolicy)
        {
            case RemovePolicy.Condition:
                if (state.CanRemove())
                    RemoveState(state);
                break;
            case RemovePolicy.WhenFailed:
                if (state.Status == RunningStatus.Failed)
                    RemoveState(state);
                break;
            case RemovePolicy.WhenSucceed:
                if (state.Status == RunningStatus.Succeed)
                    RemoveState(state);
                break;
            case RemovePolicy.WhenEnterFailed:
                if (state.CanEnter() == false)
                    RemoveState(state);
                break;
            case RemovePolicy.WhenExited:
                if (state.Status == RunningStatus.Succeed
                    || state.Status == RunningStatus.Failed)
                    RemoveState(state);
                break;
            case RemovePolicy.WhenSourceAgentNull:
                if (state.SourceAgent == null)
                    RemoveState(state);
                break;
            case RemovePolicy.WhenSourceStateRemoved:
                if (state.SourceState.Status == RunningStatus.Removed)
                    RemoveState(state);
                break;
            case RemovePolicy.WhenSourceStateExited:
                if (state.SourceState.Status == RunningStatus.Succeed
                    || state.SourceState.Status == RunningStatus.Failed)
                    RemoveState(state);
                break;
            case RemovePolicy.WhenSourceStateFailed:
                if (state.SourceState.Status == RunningStatus.Failed)
                    RemoveState(state);
                break;
            case RemovePolicy.WhenSourceStateSucceed:
                if (state.SourceState.Status == RunningStatus.Succeed)
                    RemoveState(state);
                break;
        }
    }
}