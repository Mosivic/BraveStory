namespace GPC.States;

public class GoalState : AbsState, IHubProvider
{
    public IHub Hub => GHub.GetIns();
}