using System.Threading;
using GPC;
using GPC.Job.Config;

public class ConditionSet(ICondition<IState>[] conditions) 
{
    public ICondition<IState>[] Conditions {get;set;} = conditions;
}