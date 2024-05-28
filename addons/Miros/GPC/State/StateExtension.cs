using System;

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
}