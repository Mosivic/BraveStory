using System;

namespace GPC.State;

public interface IState
{
    string Id { get; set; }
    string Name { get; set; }
    string Layer { get; set; }
    int Priority { get; set; }
    Type Type { get; set; }
    Status Status { get; set; }
}