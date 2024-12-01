namespace Miros.Core;

public static partial class Tags
{
    private static TagManager TagManager => TagManager.Instance;

    public static Tag Executor_MultiLayerConditionMachine { get; } = TagManager.RequestTag("Executor.MultiLayerConditionMachine");
    public static Tag Executor_Effect { get; } = TagManager.RequestTag("Executor.Effect");
}
