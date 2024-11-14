using Godot;

public partial class World : Node2D
{
    private Camera2D _camera;
    private TileMapLayer _tileMap;

    public override void _Ready()
    {
        _tileMap = GetNode<TileMapLayer>("TileMapLayers/Env");
        _camera = GetNode<Camera2D>("Player/Camera2D");

        var used = _tileMap.GetUsedRect().Grow(-1);
        var tileSize = _tileMap.TileSet.TileSize;

        _camera.LimitTop = used.Position.Y * tileSize.Y;
        _camera.LimitRight = used.End.X * tileSize.X;
        _camera.LimitBottom = used.End.Y * tileSize.Y;
        _camera.LimitLeft = used.Position.X * tileSize.X;
        _camera.ForceUpdateScroll();
    }
}