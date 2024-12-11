#if TOOLS
using Godot;
using System;
using System.Collections.Generic;

namespace Miros;
[Tool]
public partial class MirosPlugin : EditorPlugin
{
	private Dictionary<Node, Control> _propertyPanels = new Dictionary<Node, Control>();

	public MirosPlugin()
	{
		Name = "Miros";
		GD.Print("Miros plugin loaded");
	}

	public override void _EnterTree()
	{
		// 初始化插件
		GetTree().Connect("node_selected", new Callable(this, nameof(OnNodeSelected)));
	}

	public override void _ExitTree()
	{
		// 清理插件
		GetTree().Disconnect("node_selected", new Callable(this, nameof(OnNodeSelected)));
	}


}
#endif
