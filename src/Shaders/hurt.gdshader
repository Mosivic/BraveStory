shader_type canvas_item;

uniform float flash_duration = 0.1; // 闪烁持续时间
uniform float flash_intensity = 1.0; // 闪烁强度
uniform vec4 flash_color = vec4(1.0, 0.0, 0.0, 1.0); // 闪烁颜色

uniform bool is_hurt; // 是否受击


void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);

    // 如果角色受击，应用闪烁效果
    if (is_hurt) {
        float flash = abs(sin(TIME / flash_duration * 3.14159)); // 计算闪烁效果
        tex_color.rgb = mix(tex_color.rgb, flash_color.rgb, flash_intensity * flash); // 根据闪烁强度调整颜色
    }

    COLOR = tex_color;
}