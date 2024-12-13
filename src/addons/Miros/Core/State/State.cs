using System;

namespace Miros.Core;

public class State
{
    public Tag Tag { get; set; }
    public virtual Type Type => typeof(TaskBase);
    public int Priority { get; set; } = 0;
    public Agent OwnerAgent { get; set; }
    public Agent SourceAgent { get; set; }

    public TaskBase OwnerTask { get; set; }
    public TaskBase SourceTask { get; set; }

    // TODO：使用 State 内部的 Executor 类型来取代 Agent 中的管理
    public ExecutorType ExecutorType { get; set; } = ExecutorType.MultiLayerExecutor;

    public RunningStatus Status { get; set; } = RunningStatus.Null;
    public bool IsActive => Status == RunningStatus.Running;
    public RemovePolicy RemovePolicy { get; set; } = RemovePolicy.None; // 默认不移除
    public double RunningTime { get; set; } = 0;

    public Func<State, bool> EnterCondition { get; set; }
    public Func<State, bool> ExitCondition { get; set; }
    public Func<State, bool> RemoveCondition { get; set; }

    public Action<State> EnterFunc { get; set; }
    public Action<State> ExitFunc { get; set; }
    public Action<State> SucceedFunc { get; set; }
    public Action<State> FailedFunc { get; set; }

    public Action<State> AddFunc { get; set; } // 添加至 Executor 时
    public Action<State> RemoveFunc { get; set; } // 从 Executor 移除时

    public Action<State, double> UpdateFunc { get; set; }
    public Action<State, double> PhysicsUpdateFunc { get; set; }
}