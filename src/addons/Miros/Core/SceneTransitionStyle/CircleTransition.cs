using Godot;
using System.Threading.Tasks;

public partial class CircleTransition : CanvasLayer,ISceneTransition
{
    private AnimationPlayer _animationPlayer;
    private ColorRect _overlay;
    
    [Export]
    public float Smoothness { get; set; } = 0.1f;
    
    [Export]
    public Vector2 Center { get; set; } = new Vector2(0.5f, 0.5f);

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _overlay = GetNode<ColorRect>("ColorRect");
        
        // 设置着色器参数
        var material = _overlay.Material as ShaderMaterial;
        if (material != null)
        {
            material.SetShaderParameter("smoothness", Smoothness);
            material.SetShaderParameter("center", Center);
        }
    }

    /// <summary>
    /// 设置圆形扩散的中心点（屏幕坐标）
    /// </summary>
    public void SetTransitionCenter(Vector2 screenPosition)
    {
        var viewportSize = GetViewport().GetVisibleRect().Size;
        var normalizedPosition = new Vector2(
            screenPosition.X / viewportSize.X,
            screenPosition.Y / viewportSize.Y
        );
        
        var material = _overlay.Material as ShaderMaterial;
        if (material != null)
        {
            material.SetShaderParameter("center", normalizedPosition);
        }
    }

    public async Task TransitionOut()
    {
        _animationPlayer.Play("circle_out");
        await ToSignal(_animationPlayer, "animation_finished");
    }

    public async Task TransitionIn()
    {
        _animationPlayer.Play("circle_in");
        await ToSignal(_animationPlayer, "animation_finished");
    }
} 