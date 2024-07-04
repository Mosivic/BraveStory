using GPC;

namespace BraveStory.Scripts;

public static class LayerMap
{
    public static Layer Root = new Layer("Root", null);
    public static Layer Behavior = new Layer("Behavior", Root);
    public static Layer Buff = new("Buff", Root);
}