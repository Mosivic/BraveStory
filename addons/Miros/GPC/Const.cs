namespace GPC;

public enum JobRunningStatus
{
    NoRunning,
    Running
}

public enum JobRunningResult
{
    NoResult,
    Succeed,
    Failed
}

internal enum MsyType
{
    Property,
    Memory,
    Result,
    Sound
}

internal enum MsgState
{
    Accepted,
    Refused,
    NotFound
}