﻿using System;
using FSM.Job;
using FSM.States;

namespace FSM.Scheduler;

public interface IScheduler
{
    void AddJob(IJob job);
    void RemoveJob(IJob job);
    AbsState GetNowState(GameplayTag Layer);
    AbsState GetLastState(GameplayTag Layer);
    bool HasJobRunning(IJob job);
    void Update(double delta);
    void PhysicsUpdate(double delta);
}