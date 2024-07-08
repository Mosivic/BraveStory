namespace GPC;

public enum CompareType
{
    Equals,
    Greater,
    Less
}

public enum JobRunningStatus
{
    NoRun,
    Running,
    Succeed,
    Failed
}

public enum StateStackType
{
    None,
    Source, //对Buff Source 限制层数
    Target, //对Buff Target 限制层数
}