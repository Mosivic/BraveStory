using FSM;

namespace BraveStory.Scripts;

public static class LayerMap
{
    public static readonly Layer Movement = new("Movement", null);
    public static readonly Layer Buff = new("Buff", null, 10);

    public static readonly Layer Root = new("Root", [Movement, Buff]);
}