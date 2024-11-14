using System.Threading.Tasks;
using Godot;

public partial class PixelateTransition : CanvasLayer, ISceneTransition
{
    private AnimationPlayer _animationPlayer;
    private ColorRect _overlay;

    [Export] public float PixelSize { get; set; } = 100.0f;

    public async Task TransitionOut()
    {
        _animationPlayer.Play("pixelate_out");
        await ToSignal(_animationPlayer, "animation_finished");
    }

    public async Task TransitionIn()
    {
        _animationPlayer.Play("pixelate_in");
        await ToSignal(_animationPlayer, "animation_finished");
    }

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _overlay = GetNode<ColorRect>("ColorRect");

        // 设置像素化参数
        var material = _overlay.Material as ShaderMaterial;
        if (material != null) material.SetShaderParameter("pixels", PixelSize);
    }
}