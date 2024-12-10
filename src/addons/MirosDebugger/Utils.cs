using Godot;

namespace MirosDebugger;


[Tool]
public static class Utils
{
	public static EditorPlugin GetPlugin()
	{
		var tree = Engine.GetMainLoop() as SceneTree;
		var plugin = tree.Root.GetNodeOrNull<EditorPlugin>("MirosDebuggerPlugin");
		return plugin;
	}

	public static float GetEditorScale()
	{
		var plugin = GetPlugin();
		if (plugin == null)
			return 1.0f;
		return EditorInterface.Singleton.GetEditorScale();
	}


	// BUG: 这个方法在编辑器中返回 null
	public static RefCounted GetFrames()
	{
		var plugin = GetPlugin();
		if (plugin == null)
			return null;
		return null;
	}
}

