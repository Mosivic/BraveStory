using System;
using System.Collections.Generic;

namespace GPC.Job.Config;

public static class StateExtension
{
    public static void SetArg(this State state, string key, object value)
    {
        state.Args[key] = value;
    }

    public static dynamic GetArg(this State state, string key)
    {
        if (state.Args.TryGetValue(key, out var arg)) return arg;
        throw new Exception($"GPC.Job.Status.GetArg(): Not found key: {key} in state");
    }

    public static bool IsAllConditionSatisfy(this State state, Dictionary<object, bool> conditions)
    {
        foreach (var key in conditions.Keys)
            switch (key)
            {
                case string:
                    bool arg = state.GetArg(key as string);
                    if (arg != conditions[key]) return false;
                    break;
                case ICondition:
                    if (!(key as ICondition).IsSatisfy(state)) return false;
                    break;
                default:
                    return false;
            };
        return true;
    }

    public static bool IsAnyConditionSatisfy(this State state, Dictionary<object, bool> conditions)
    {
        foreach (var key in conditions.Keys)
            switch (key)
            {
                case string:
                    bool arg = state.GetArg(key as string);
                    if (arg == conditions[key]) return true;
                    break;
                case ICondition:
                    if ((key as ICondition).IsSatisfy(state)) return true;
                    break;
                default:
                    return false;
            };
        return false;
    }
}