namespace GPC;

public class Layer(string name, Layer parentLayer)
{
    public string Name { get; set; } = name;
    public Layer ParentLayer { get; set; } = parentLayer;
}