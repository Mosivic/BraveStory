﻿using System;
using System.Collections.Generic;

namespace GPC.Job.Config;

internal enum DurationPolicy //持续政策
{
    Instand, //立即生效
    Infinite, //永久生效
    Interval //规定时长
}

internal enum PeriodicInhibitionPolicy //Buff中断并恢复的处理政策
{
    Resume, //恢复衔接
    Reset, //重置
    ExecuteAndReset //执行一次后再重置
}

internal enum StackDurationRefreshPolicy //叠加新buff时持续时间的更新政策
{
    Reset, //重置
    Delay //延长
}

internal enum StackPeriodResetPolicy ////叠加新buff时周期的更新政策
{
    Reset,
    Delay
}

internal enum StackExpirationPolicy //当一层buff到期后的处理政策
{
    ClearAllStack,
    RemoveOneStackAndRefreshDuration,
    RefreshDuration
}

internal struct BuffArgs
{
    public float LeftTime;
}

internal class Buff : IState
{
    public string Id { get; set; }
    public string Layer { get; set; }
    public Type Type { get; set; }
    public Status Status { get; set; }
    
    private BuffArgs args;
    private List<Buff> FailedBuffs { get; set; }= null;

    private List<Buff> overflowBuffs = null; //层数溢出后调用的Buff
    private float period = 0;
    private PeriodicInhibitionPolicy periodicInhibitionPolicy = PeriodicInhibitionPolicy.Resume;
    private int stackMaxCount = 1;
    private List<Buff> succeedBuffs = null;

    public DurationPolicy DurationPolicy { get; set; } = DurationPolicy.Instand;

}