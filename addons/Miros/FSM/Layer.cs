using System.Collections.Generic;

namespace FSM;

public class Layer(string name, List<Layer> childrenLayer, int onRunningJobMaxCount = 1)
{
    public string Name { get; set; } = name;
    public int OnRunningJobMaxCount { get; set; } = onRunningJobMaxCount;
    public List<Layer> ChildrenLayer { get; set; } = childrenLayer;
    public List<string> ChildrenName { get; set; }

    public void RegisterChild(string name)
    {
        if (ChildrenName.Contains(name)) return;
        ChildrenName.Add(name);
    }

    public void UnregisterChild(string name)
    {
        ChildrenName.Remove(name);
    }
}