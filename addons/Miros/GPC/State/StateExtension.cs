using System;
using System.Collections.Generic;

namespace GPC.Job.Config;

public static class StateExtension
{
    private static List<IJob<IState>> _jobs = new List<IJob<IState>>();
    
    
    public static void SetArg(this State state, string key, object value)
    {
        state.Args[key] = value;
    }

    public static dynamic GetArg(this State state, string key)
    {
        if (state.Args.TryGetValue(key, out var arg)) return arg;
        throw new Exception($"GPC.Job.Status.GetArg(): Not found key: {key} in state");
    }

    public static bool IsAllConditionSatisfy(this State state, Dictionary<ICondition, bool> conditions)
    {
        foreach (var condition in conditions.Keys)
            if (condition.IsSatisfy(state) != conditions[condition])
                return false;
        return true;
    }

    public static bool IsAnyConditionSatisfy(this State state, Dictionary<ICondition, bool> conditions)
    {
        foreach (var condition in conditions.Keys)
            if (condition.IsSatisfy(state) == conditions[condition])
                return true;
        return false;
    }
}