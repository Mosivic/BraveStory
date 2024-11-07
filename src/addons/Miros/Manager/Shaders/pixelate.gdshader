shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0) = 0.0;
uniform float pixels = 100.0;

void fragment() {
    // 计算像素大小
    float pixel_size = mix(1.0, pixels, progress);
    
    // 计算像素化后的UV坐标
    vec2 grid_uv = round(UV * pixel_size) / pixel_size;
    
    // 采样原始颜色
    COLOR = texture(TEXTURE, grid_uv);
} 