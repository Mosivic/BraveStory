using System;

namespace GPC.Job.Config;

public class Goal : IState, IHubProvider
{
    public IHub Hub => GHub.GetIns();
    public string Id { get; set; }
    public string Name { get; set; }
    public string Layer { get; set; }
    public Type Type { get; set; }
    public Status Status { get; set; }
    public int Priority { get; set; }
}