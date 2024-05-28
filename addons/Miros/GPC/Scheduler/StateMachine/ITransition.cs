﻿using GPC.Job.Config;

namespace GPC.AI.StateMachine;

public interface ITransition<T> where T : IState
{
    T To { get; }
    Condition<T> Condition { get; }
}