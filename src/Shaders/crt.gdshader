shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

// 基础效果
uniform float scan_line_count : hint_range(0, 1080) = 180.0;
uniform float scan_line_intensity : hint_range(0.0, 1.0) = 0.1;
uniform float chromatic_aberration : hint_range(0.0, 5.0) = 0.2;
uniform float vignette_strength : hint_range(0.0, 1.0) = 0.2;
uniform float distortion : hint_range(0.0, 0.2) = 0.02;

// 新增效果参数
uniform float brightness : hint_range(0.0, 2.0) = 1.2;          // 亮度
uniform float blur_amount : hint_range(0.0, 2.0) = 0.3;         // 模糊程度
uniform float flicker_intensity : hint_range(0.0, 1.0) = 0.05;  // 闪烁强度
uniform float rgb_mask_strength : hint_range(0.0, 1.0) = 0.3;   // RGB掩码强度
uniform float grain_amount : hint_range(0.0, 0.5) = 0.05;       // 颗粒感
uniform float ghost_amount : hint_range(0.0, 1.0) = 0.1;        // 重影强度
uniform float glow_amount : hint_range(0.0, 2.0) = 0.3;         // 发光强度

// 时间变量用于动画效果
uniform float time_offset;

// 随机数生成
float rand(vec2 co) {
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

// 噪声纹理
float noise(vec2 p) {
    vec2 ip = floor(p);
    vec2 u = fract(p);
    u = u*u*(3.0-2.0*u);
    
    float res = mix(
        mix(rand(ip), rand(ip+vec2(1.0,0.0)), u.x),
        mix(rand(ip+vec2(0.0,1.0)), rand(ip+vec2(1.0,1.0)), u.x), u.y);
    return res*res;
}

vec2 curve(vec2 uv) {
    uv = uv * 2.0 - 1.0;
    vec2 offset = abs(uv.yx) / vec2(6.0, 4.0);
    uv = uv + uv * offset * offset * distortion;
    uv = uv * 0.5 + 0.5;
    return uv;
}

// RGB掩码效果
vec4 apply_rgb_mask(vec2 uv, vec4 col) {
    float mask_r = mod(uv.x * 3.0 + uv.y * 3.0, 3.0);
    float mask_g = mod(uv.x * 3.0 + uv.y * 3.0 + 1.0, 3.0);
    float mask_b = mod(uv.x * 3.0 + uv.y * 3.0 + 2.0, 3.0);
    
    vec3 mask = vec3(mask_r, mask_g, mask_b);
    mask = clamp(mask, 0.0, 1.0);
    
    return vec4(col.rgb * (1.0 - rgb_mask_strength + mask * rgb_mask_strength), col.a);
}

// 发光效果
vec4 apply_glow(vec2 uv, vec4 col) {
    vec4 glow = vec4(0.0);
    float total = 0.0;
    
    for(float i = -2.0; i <= 2.0; i++) {
        for(float j = -2.0; j <= 2.0; j++) {
            vec2 offset = vec2(i, j) * 0.01;
            float weight = 1.0 - length(offset) * 0.5;
            if(weight > 0.0) {
                glow += texture(SCREEN_TEXTURE, uv + offset) * weight;
                total += weight;
            }
        }
    }
    
    glow /= total;
    return mix(col, glow, glow_amount);
}

void fragment() {
    vec2 uv = curve(SCREEN_UV);
    vec4 color;
    
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) {
        color = vec4(0.0, 0.0, 0.0, 1.0);
    } else {
        // 动画效果1：移动的扫描线
        float scan_line = sin((uv.y + time_offset * 2.0) * scan_line_count * 3.14159);
        
        // 动画效果2：闪烁效果
        float flicker = 1.0 + sin(time_offset * 10.0) * flicker_intensity;
        
        // 动画效果3：波动的色差
        float aberration = chromatic_aberration * (1.0 + sin(time_offset * 3.0) * 0.2);
        float r = texture(SCREEN_TEXTURE, vec2(uv.x + aberration * 0.01, uv.y)).r;
        float g = texture(SCREEN_TEXTURE, uv).g;
        float b = texture(SCREEN_TEXTURE, vec2(uv.x - aberration * 0.01, uv.y)).b;
        
        color = vec4(r, g, b, 1.0);
        
        // 动画效果4：移动的噪点
        float noise = rand(uv + vec2(time_offset)) * grain_amount;
        
        // 动画效果5：脉冲式的发光
        float glow_pulse = glow_amount * (1.0 + sin(time_offset * 2.0) * 0.3);
        
        // 应用效果
        color.rgb *= 1.0 - (scan_line * scan_line_intensity);  // 扫描线
        color.rgb *= flicker;  // 闪烁
        color.rgb += noise;    // 噪点
        
        // 动画效果6：移动的重影
        vec2 ghost_offset = vec2(
            sin(time_offset) * 0.01,
            cos(time_offset) * 0.01
        );
        vec4 ghost = texture(SCREEN_TEXTURE, uv + ghost_offset);
        color.rgb = mix(color.rgb, ghost.rgb, ghost_amount);
        
        // 动画效果7：呼吸式的暗角
        float vignette_pulse = vignette_strength * (1.0 + sin(time_offset) * 0.2);
        float vignette = uv.x * uv.y * (1.0 - uv.x) * (1.0 - uv.y);
        vignette = pow(vignette * 15.0, vignette_pulse);
        color.rgb *= vignette;
        
        // 应用最终的亮度
        color.rgb *= brightness;
    }
    
    COLOR = color;
}