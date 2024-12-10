#if TOOLS
using Godot;
using System;
using System.Collections.Generic;

namespace MirosDebugger;
[Tool]
public partial class MirosDebugger : EditorPlugin
{
	private Dictionary<Node, Control> _propertyPanels = new Dictionary<Node, Control>();

	public MirosDebugger()
	{
		Name = "MirosDebugger";
		GD.Print("MirosDebugger plugin loaded");
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

	private void OnNodeSelected(Node node)
	{
		// 显示选中节点的属性
		if (node is Node2D || node is Control)
		{
			ShowProperties(node);
		}
	}

	private void ShowProperties(Node node)
	{
		// 创建或更新属性面板
		if (!_propertyPanels.ContainsKey(node))
		{
			Control panel = new Control(); // 创建新的面板
			_propertyPanels[node] = panel;
			AddChild(panel); // 直接将面板添加到插件中
		}

		// 更新面板内容
		UpdatePropertyPanel(_propertyPanels[node], node);
	}

	private void UpdatePropertyPanel(Control panel, Node node)
	{
		// 清空面板并添加属性
		foreach (var child in panel.GetChildren())
		{
			panel.RemoveChild(child); // 移除所有子节点
		}

		foreach (var property in node.GetPropertyList())
		{
			// 创建 UI 元素以显示和编辑属性
			Label label = new Label { Text = property.ToString() };
			panel.AddChild(label);
			// 这里可以添加更多的 UI 元素来编辑属性
		}
	}
}
#endif
