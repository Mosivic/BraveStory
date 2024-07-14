﻿namespace FSM.Job;

public interface IJob
{
    string Name { get; }
    Layer Layer { get; }
    int Priority { get; }
    JobRunningStatus Status { get; }
    bool IsStack { get; }
    object Source { get; }
    void Enter();
    void Exit();
    void Pause();
    void Resume();
    void Stack(object source);
    bool CanEnter();
    bool CanExit();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}