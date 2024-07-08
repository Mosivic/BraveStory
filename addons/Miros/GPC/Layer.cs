using System.Collections.Generic;

namespace GPC;

public class Layer(string name, List<Layer> childrenLayer, int jobMaxCount = 1)
{
    public string Name { get; set; } = name;

    public int JobMaxCount { get; set; } = jobMaxCount;
    public List<Layer> ChildrenLayer { get; set; } = childrenLayer;
}