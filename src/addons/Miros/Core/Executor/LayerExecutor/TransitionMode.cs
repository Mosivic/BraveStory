namespace Miros.Core;

public enum TransitionMode
{
    None, // 无
    Normal, // 正常,当退出条件、转换条件、进入条件都满足 -> 转换状态
    Force, // 强制，当转换条件、进入条件满足 -> 转换状态
    DelayFront, // 延迟，当转换条件、进入条件满足后等待退出条件满足 -> 转换条件
    DelayBackend // 延迟，当退出条件、转换条件满足后等待进入条件满足 -> 转换条件
}