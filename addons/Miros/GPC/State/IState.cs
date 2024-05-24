using System;

namespace GPC.Job.Config;

public interface IState
{
    string Id { get; set; }
    string Layer { get; set; }
    Type Type { get; set; }
    Status Status { get; set; }
}