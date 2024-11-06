using Godot;
using System.Threading.Tasks;

public partial class RainTransition : CanvasLayer,ISceneTransition
{
    private AnimationPlayer _animationPlayer;
    private ColorRect _overlay;
    
    [Export]
    public float DistortionStrength { get; set; } = 0.2f;
    
    [Export]
    public float RippleSpeed { get; set; } = 3.0f;
    
    [Export]
    public float FlowSpeed { get; set; } = 1.0f;
    
    [Export]
    public float DropRate { get; set; } = 0.7f;

    private void UpdateParameters()
    {
        var material = _overlay.Material as ShaderMaterial;
        if (material != null)
        {
            material.SetShaderParameter("distortion_strength", DistortionStrength);
            material.SetShaderParameter("ripple_speed", RippleSpeed);
            material.SetShaderParameter("flow_speed", FlowSpeed);
            material.SetShaderParameter("drop_rate", DropRate);
        }
    }

    /// <summary>
    /// 设置雨滴效果参数
    /// </summary>
    public void SetRainParameters(
        float? distortion = null,
        float? rippleSpeed = null,
        float? flowSpeed = null,
        float? dropRate = null)
    {
        if (distortion.HasValue) DistortionStrength = distortion.Value;
        if (rippleSpeed.HasValue) RippleSpeed = rippleSpeed.Value;
        if (flowSpeed.HasValue) FlowSpeed = flowSpeed.Value;
        if (dropRate.HasValue) DropRate = dropRate.Value;
        
        UpdateParameters();
    }

    public async Task TransitionOut()
    {
        _animationPlayer.Play("rain_out");
        await ToSignal(_animationPlayer, "animation_finished");
    }

    public async Task TransitionIn()
    {
        _animationPlayer.Play("rain_in");
        await ToSignal(_animationPlayer, "animation_finished");
    }
    

} 