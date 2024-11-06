shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0) = 0.0;
uniform float smoothness : hint_range(0.0, 1.0) = 0.01;
uniform vec2 direction = vec2(1.0, 0.0); // 滑动方向

void fragment() {
    // 计算滑动边缘位置
    float slide_edge = dot(UV - vec2(0.5), normalize(direction)) + 0.5;
    
    // 创建平滑的边缘过渡
    float alpha = smoothstep(progress, progress + smoothness, slide_edge);
    
    // 输出黑色，使用计算的alpha值
    COLOR = vec4(0.0, 0.0, 0.0, alpha);
} 