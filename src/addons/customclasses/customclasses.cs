#if TOOLS
using Godot;
using System;

[Tool]
public partial class customclasses : EditorPlugin
{
	public override void _EnterTree()
	{
		AddCustomType("AgentNode", "Node", GD.Load<Script>("res://addons/Miros/Core/Agent/AgentNode.cs"), null);
		AddCustomType("StateNode", "Node", GD.Load<Script>("res://addons/Miros/Core/State/StateNode.cs"), null);
	}

	public override void _ExitTree()
	{
		RemoveCustomType("AgentNode");
		RemoveCustomType("StateNode");
	}
}
#endif
