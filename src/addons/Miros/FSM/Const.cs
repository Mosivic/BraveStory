namespace FSM;

public enum CompareType
{
    Equals,
    Greater,
    Less
}

public enum RunningStatus
{
    NoRun,
    Running,
    Succeed,
    Failed
}

public enum StateStackType
{
    Source, //对Buff Source 限制层数
    Target //对Buff Target 限制层数
}
