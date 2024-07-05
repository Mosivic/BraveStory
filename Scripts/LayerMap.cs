using GPC;

namespace BraveStory.Scripts;

public static class LayerMap
{
    public static readonly Layer Root = new Layer("Root", null);
    public static readonly Layer Behavior = new Layer("Behavior", Root);
    public static readonly Layer Buff = new("Buff", Root);
}