using System.Threading.Tasks;
using Godot;

public partial class FadeTransition : CanvasLayer, ISceneTransition
{
    private AnimationPlayer _animationPlayer;
    private ColorRect _overlay;

    /// <summary>
    ///     播放淡出动画（场景变黑）
    /// </summary>
    public async Task TransitionOut()
    {
        _animationPlayer.Play("fade_out");
        await ToSignal(_animationPlayer, "animation_finished");
    }

    /// <summary>
    ///     播放淡入动画（显示新场景）
    /// </summary>
    public async Task TransitionIn()
    {
        _animationPlayer.Play("fade_in");
        await ToSignal(_animationPlayer, "animation_finished");
    }

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _overlay = GetNode<ColorRect>("ColorRect");

        // 初始状态设置为透明
        _overlay.Modulate = new Color(1, 1, 1, 0);
    }
}