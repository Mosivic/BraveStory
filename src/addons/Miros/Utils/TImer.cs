﻿using System;

namespace Miros.Utils;

public abstract class Timer(double time)
{
    protected double initialTime = time;

    public Action OnTimerStart;
    public Action OnTimerStop;
    public double Time { get; set; }
    public bool IsRunning { get; protected set; }

    public double Progress => Time / initialTime;

    public void Start()
    {
        Time = initialTime;

        if (IsRunning) return;
        IsRunning = true;
        OnTimerStart?.Invoke();
    }

    public void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        OnTimerStop?.Invoke();
    }


    public void Resume()
    {
        IsRunning = true;
    }

    public void Pause()
    {
        IsRunning = false;
    }

    public abstract void Tick(double deltaTime);
}

public class CounterTimer(double time) : Timer(time)
{
    public bool IsFinished => Time <= 0;

    public override void Tick(double deltaTime)
    {
        // 防止误差
        if (IsRunning && Time < double.Epsilon) Stop();

        if (IsRunning && Time > 0) Time -= deltaTime;

        if (IsRunning && Time < 0) Stop();
    }

    public void Reset()
    {
        Time = initialTime;
    }

    public void Reset(float time)
    {
        Time = initialTime = time;
    }
}

public class StopwatchTimer : Timer
{
    public StopwatchTimer(double time) : base(time)
    {
    }

    public override void Tick(double deltaTime)
    {
        if (IsRunning) Time += deltaTime;
    }

    public void Reset()
    {
        Time = 0;
    }

    public double GetTime()
    {
        return Time;
    }
}