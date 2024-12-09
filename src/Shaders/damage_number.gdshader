shader_type canvas_item;

uniform float flash_duration : hint_range(0.0, 1.0) = 0.1; // 闪烁持续时间
uniform float shake_intensity : hint_range(0.0, 10.0) = 5.0; // 震动强度
uniform float lifetime : hint_range(0.0, 5.0) = 2.0; // 生命周期
uniform vec4 start_color = vec4(1.0, 0.0, 0.0, 1.0); // 开始颜色
uniform vec4 end_color = vec4(1.0, 1.0, 1.0, 1.0); // 结束颜色

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    
    // 计算闪烁效果
    float flash = abs(sin(TIME / flash_duration * 3.14159)); // 计算闪烁效果
    tex_color.rgb *= mix(start_color.rgb, end_color.rgb, flash); // 根据闪烁强度调整颜色

    // 计算震动效果
    vec2 shake = vec2(sin(TIME * 20.0) * shake_intensity, cos(TIME * 20.0) * shake_intensity);
    vec2 new_uv = UV + shake; // 使用震动偏移 UV 坐标
    tex_color = texture(TEXTURE, new_uv); // 使用新的 UV 坐标获取纹理颜色

    // 设置最终颜色
    COLOR = tex_color;
}