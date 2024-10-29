using Godot;

public partial class Test : Node2D
{
    private Camera2D _camera;
    private TileMap _tileMap;

    public override void _Ready()
    {
        _tileMap = GetNode<TileMap>("map");
        _camera = GetNode<Camera2D>("Player/Camera2D");

        var used = _tileMap.GetUsedRect().Grow(-1);
        var tileSize = _tileMap.TileSet.TileSize;

        _camera.LimitTop = used.Position.Y * tileSize.Y;
        _camera.LimitRight = used.End.X * tileSize.X;
    }
}