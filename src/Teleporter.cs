using Godot;

public partial class Teleporter:Interactable
{
    [Export]
    public string path;

    public override void Interact()
    {
        base.Interact();
        GetTree().ChangeSceneToFile(path);
    }
}

