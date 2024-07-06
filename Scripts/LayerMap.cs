using GPC;

namespace BraveStory.Scripts;

public static class LayerMap
{
    public static readonly Layer Root = new("Root", null);
    public static readonly Layer Movement = new("Movement", Root);
    public static readonly Layer Buff = new("Buff", Root);
}