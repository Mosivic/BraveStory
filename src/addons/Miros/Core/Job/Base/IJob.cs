﻿namespace Miros.Core;

public interface IJob
{
    void Enter();
    void Exit();
    void Stack();
    void Deactivate(); // 停用
    void Activate(); // 启用
    bool CanEnter();
    bool CanExit();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}