using System;
using Godot;
using GPC.Scheduler;

namespace GPC.State;

public interface IState
{
    string Id { get; set; }
    string Name { get; set; }
    string Layer { get; set; }
    int Priority { get; set; }
    Node Host {get;set;}
    AbsScheduler<IState> Scheduler {get;set;}
    Type Type { get; set; }
    Status Status { get; set; }

}