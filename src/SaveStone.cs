using Godot;

public partial class SaveStone : Interactable
{
    private AnimationPlayer _animationPlayer;


    public override void _Ready()
    {
        base._Ready();
        // 获取 AnimationPlayer 节点引用
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void Interact()
    {
        // 添加空值检查
        if (_animationPlayer != null)
            _animationPlayer.Play("activated");
        else
            GD.PrintErr("AnimationPlayer 未找到!");

        SaveManager.Instance.SaveGame("test");
    }
}