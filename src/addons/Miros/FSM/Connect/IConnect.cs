﻿using FSM.States;

namespace FSM.Job.Executor;

public interface IConnect
{
    void AddState(AbsState state);
    void RemoveState(AbsState state);
    void Update(double delta);
    void PhysicsUpdate(double delta);
    IJob[] GetAllJobs();
}