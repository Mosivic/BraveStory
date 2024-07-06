using System.Collections.Generic;

namespace GPC;

public class Layer(string name, List<Layer> childrenLayer)
{
    public string Name { get; set; } = name;
    public List<Layer> ChildrenLayer { get; set; } = childrenLayer;
}