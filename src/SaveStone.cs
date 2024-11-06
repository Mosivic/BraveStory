using Godot;
using System;

public partial class SaveStone : Interactable
{
	private AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		base._Ready();
		// 获取 AnimationPlayer 节点引用
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void Interact()
	{
		// 添加空值检查
		if (animationPlayer != null)
		{
			animationPlayer.Play("activated");
		}
		else
		{
			GD.PrintErr("AnimationPlayer 未找到!");
		}
		
		SaveManager.Instance.SaveGame("test");
	}
}
