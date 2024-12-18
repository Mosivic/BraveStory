using System;

namespace Miros.Core;

public class State
{
    public virtual Tag Tag { get; set; }
    public virtual TaskType TaskType { get; set; } = TaskType.Base;
    public int Priority { get; set; } = 0;
    public Agent OwnerAgent { get; set; }
    public Agent SourceAgent { get; set; }

    public ITask Task { get; set; }

    public State SourceState { get; set; }

    // TODO：使用 State 内部的 Executor 类型来取代 Agent 中的管理
    public ExecutorType ExecutorType { get; set; } = ExecutorType.MultiLayerExecutor;

    public RunningStatus Status { get;  set; } = RunningStatus.Null;
    public bool IsActive => Status == RunningStatus.Running;
    public RemovePolicy RemovePolicy { get; set; } = RemovePolicy.None; // 默认不移除
    public double RunningTime { get; set; } = 0;

    public Func<bool> EnterCondition { get; set; }
    public Func<bool> ExitCondition { get; set; }
    public Func<bool> RemoveCondition { get; set; }

    public Action EnterFunc { get; set; }
    public Action ExitFunc { get; set; }
    public Action SucceedFunc { get; set; }
    public Action FailedFunc { get; set; }

    public Action AddFunc { get; set; } // 添加至 Executor 时
    public Action RemoveFunc { get; set; } // 从 Executor 移除时

    public Action<double> UpdateFunc { get; set; }
    public Action<double> PhysicsUpdateFunc { get; set; }
}