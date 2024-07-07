namespace GPC;

public enum CompareType
{
    Equals,
    Greater,
    Less
}

public enum JobRunningStatus
{
    Enter,
    Update,
    Exit,
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