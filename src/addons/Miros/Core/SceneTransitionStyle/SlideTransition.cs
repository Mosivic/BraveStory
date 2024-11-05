using Godot;
using System.Threading.Tasks;

public partial class SlideTransition : CanvasLayer,ISceneTransition
{
    private AnimationPlayer _animationPlayer;
    private ColorRect _overlay;
    
    public enum SlideDirection
    {
        Left,
        Right,
        Up,
        Down
    }
    
    [Export]
    public SlideDirection Direction { get; set; } = SlideDirection.Right;
    
    [Export]
    public float Smoothness { get; set; } = 0.01f;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _overlay = GetNode<ColorRect>("ColorRect");
        
        UpdateDirection();
    }
    
    /// <summary>
    /// 设置滑动方向
    /// </summary>
    public void SetDirection(SlideDirection direction)
    {
        Direction = direction;
        UpdateDirection();
    }
    
    private void UpdateDirection()
    {
        var material = _overlay.Material as ShaderMaterial;
        if (material != null)
        {
            Vector2 dir = Direction switch
            {
                SlideDirection.Left => new Vector2(-1, 0),
                SlideDirection.Right => new Vector2(1, 0),
                SlideDirection.Up => new Vector2(0, -1),
                SlideDirection.Down => new Vector2(0, 1),
                _ => new Vector2(1, 0)
            };
            
            material.SetShaderParameter("direction", dir);
            material.SetShaderParameter("smoothness", Smoothness);
        }
    }

    public async Task TransitionOut()
    {
        _animationPlayer.Play("slide_out");
        await ToSignal(_animationPlayer, "animation_finished");
    }

    public async Task TransitionIn()
    {
        _animationPlayer.Play("slide_in");
        await ToSignal(_animationPlayer, "animation_finished");
    }
} 