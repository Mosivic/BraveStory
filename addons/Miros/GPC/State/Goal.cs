using System;
using GPC.Scheduler;

namespace GPC.States;

public class Goal : AbsState, IHubProvider
{
    public IHub Hub => GHub.GetIns();
}