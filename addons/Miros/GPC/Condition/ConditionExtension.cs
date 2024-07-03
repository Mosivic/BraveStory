namespace GPC.Condition;

public class ConditionExtension
{
    public static void SetArg(this ICondition<T> state, string key, object value)
    {
         state.Args[key] = value;
    }
}