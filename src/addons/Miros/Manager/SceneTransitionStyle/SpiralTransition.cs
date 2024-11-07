using Godot;
using System.Threading.Tasks;

public partial class SpiralTransition : CanvasLayer,ISceneTransition
{
    private AnimationPlayer _animationPlayer;
    private ColorRect _overlay;
    
    [Export]
    public float RotationSpeed { get; set; } = 3.0f;
    
    [Export]
    public float SpiralIntensity { get; set; } = 30.0f;
    
    [Export]
    public Vector2 Center { get; set; } = new Vector2(0.5f, 0.5f);

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _overlay = GetNode<ColorRect>("ColorRect");
        
        UpdateParameters();
    }
    
    /// <summary>
    /// 设置漩涡中心点（屏幕坐标）
    /// </summary>
    public void SetTransitionCenter(Vector2 screenPosition)
    {
        var viewportSize = GetViewport().GetVisibleRect().Size;
        var normalizedPosition = new Vector2(
            screenPosition.X / viewportSize.X,
            screenPosition.Y / viewportSize.Y
        );
        
        Center = normalizedPosition;
        UpdateParameters();
    }
    
    private void UpdateParameters()
    {
        var material = _overlay.Material as ShaderMaterial;
        if (material != null)
        {
            material.SetShaderParameter("rotation_speed", RotationSpeed);
            material.SetShaderParameter("spiral_intensity", SpiralIntensity);
            material.SetShaderParameter("center", Center);
        }
    }

    public async Task TransitionOut()
    {
        _animationPlayer.Play("spiral_out");
        await ToSignal(_animationPlayer, "animation_finished");
    }

    public async Task TransitionIn()
    {
        _animationPlayer.Play("spiral_in");
        await ToSignal(_animationPlayer, "animation_finished");
    }
} 