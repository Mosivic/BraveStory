﻿using System;
using System.Collections.Generic;
using GPC.States;

namespace GPC.Scheduler;

public abstract class AbsScheduler()
{
    public Action<AbsState> StateChanged;
    public Action<AbsState> StatePrepared;
    protected Dictionary<Layer, List<AbsState>> LayerStates { get; set; } = new();
    protected readonly Dictionary<Layer, List<AbsState>> RunningLayerStates = new();
    
    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}