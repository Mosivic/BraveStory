shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0) = 0.0;
uniform float smoothness : hint_range(0.0, 1.0) = 0.1;
uniform vec2 center = vec2(0.5, 0.5);
uniform float inverted = 0.0;

void fragment() {
    // 计算当前像素到中心点的距离
    float dist = distance(UV, center);
    
    // 创建圆形mask
    float circle = smoothstep(progress, progress + smoothness, dist);
    
    // 是否反转效果（用于淡入）
    circle = inverted > 0.5 ? 1.0 - circle : circle;
    
    // 输出黑色，使用circle作为alpha值
    COLOR = vec4(0.0, 0.0, 0.0, circle);
} 