using Godot;

public partial class Teleporter : Interactable
{
	[Export] public string Path { get; set; }
	[Export] public Vector2 EntryPoint { get; set; }
	[Export] public Node2D Player { get; set; }


	public override void Interact()
	{
		base.Interact();

		if (string.IsNullOrEmpty(Path))
		{
			GD.PrintErr("Scene path is not set!");
			return;
		}

		// 如果 Player 为空，重新尝试获取
		if (Player == null)
		{
			GD.PrintErr("Player reference is null and could not be found!");
			return;
		}

		SceneManager.Instance?.ChangeScene(Path, Player, EntryPoint);
	}
}
