shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0) = 0.0;
uniform float rotation_speed = 3.0;
uniform float spiral_intensity = 30.0;
uniform vec2 center = vec2(0.5, 0.5);
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
    // 计算相对于中心的位置
    vec2 rel_pos = UV - center;
    float dist = length(rel_pos);
    
    // 计算旋转角度
    float angle = atan(rel_pos.y, rel_pos.x);
    float spiral = angle + dist * spiral_intensity * progress;
    
    // 添加旋转动画
    spiral += progress * rotation_speed;
    
    // 计算扭曲后的UV坐标
    vec2 spiral_uv = center + vec2(
        cos(spiral) * dist,
        sin(spiral) * dist
    );
    
    // 混合原始图像和扭曲后的图像
    vec4 original = texture(SCREEN_TEXTURE, UV);
    vec4 distorted = texture(SCREEN_TEXTURE, spiral_uv);
    
    // 添加渐隐效果
    float fade = smoothstep(0.0, 1.0, progress);
    
    // 输出最终颜色
    COLOR = mix(original, distorted, progress);
    COLOR.a = fade;
} 