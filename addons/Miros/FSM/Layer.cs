using System.Collections.Generic;
using FSM.Job;

namespace FSM;

public class Layer(string name, List<Layer> childrenLayer, int onRunningJobMaxCount = 1)
{
    public string Name { get; set; } = name;
    public int OnRunningJobMaxCount { get; set; } = onRunningJobMaxCount;
    public List<Layer> ChildrenLayer { get; set; } = childrenLayer;
    public List<string> ChildrenName { get; set; }

    public void RegisterChild(string name)
    {
        if(ChildrenName.Contains(name)) return;
        ChildrenName.Add(name);
    }
    public void UnRegisterChild(string name)
    {
        ChildrenName.Remove(name);
    }
    
}