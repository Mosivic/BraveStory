shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0) = 0.0;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform float distortion_strength = 0.2;
uniform float ripple_speed = 3.0;
uniform float flow_speed = 1.0;
uniform float drop_rate = 0.7;

// 噪声函数
float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898,78.233))) * 43758.5453123);
}

// 生成雨滴碰撞涟漪
vec2 ripple(vec2 uv, vec2 center, float time, float size) {
    vec2 delta = uv - center;
    float dist = length(delta);
    
    // 涟漪扩散
    float ripple = sin(dist * 50.0 - time * ripple_speed) * exp(-dist * 8.0);
    
    // 随时间衰减
    ripple *= smoothstep(1.0, 0.0, time);
    
    return delta * ripple * distortion_strength;
}

// 生成雨滴流动轨迹
vec2 flow_trail(vec2 uv, vec2 start_pos, float time) {
    vec2 pos = start_pos;
    float strength = 1.0;
    vec2 distortion = vec2(0.0);
    
    // 模拟重力流动
    for(float i = 0.0; i < 10.0; i++) {
        float t = time - i * 0.1;
        if(t < 0.0) break;
        
        // 计算流动路径
        pos.y += t * flow_speed;
        pos.x += sin(t * 2.0 + start_pos.x * 10.0) * 0.02;
        
        // 计算扭曲强度
        float trail_strength = exp(-i * 0.3) * strength;
        
        // 添加扭曲
        vec2 delta = uv - pos;
        float dist = length(delta);
        distortion += normalize(delta) * exp(-dist * 10.0) * trail_strength;
    }
    
    return distortion * distortion_strength;
}

void fragment() {
    vec2 uv = UV;
    vec2 total_distortion = vec2(0.0);
    
    // 生成多个雨滴
    for(float i = 0.0; i < 20.0; i++) {
        // 随机位置和时间偏移
        vec2 drop_pos = vec2(
            random(vec2(i, 1.234)),
            random(vec2(i, 5.678))
        );
        float time_offset = random(vec2(i, 9.012));
        float drop_time = fract(progress * drop_rate + time_offset);
        
        // 添加涟漪扭曲
        total_distortion += ripple(uv, drop_pos, drop_time, 0.1);
        
        // 添加流动轨迹
        if(drop_time > 0.1) {
            total_distortion += flow_trail(uv, drop_pos, drop_time);
        }
    }
    
    // 应用扭曲到屏幕纹理
    vec2 distorted_uv = uv + total_distortion;
    vec4 screen_color = texture(SCREEN_TEXTURE, distorted_uv);
    
    // 添加水滴高光效果
    float highlight = length(total_distortion) * 2.0;
    vec4 water_color = vec4(1.0) * highlight;
    
    // 混合最终颜色
    COLOR = mix(screen_color, water_color, highlight * 0.5);
    
    // 添加整体渐变
    float fade = smoothstep(0.0, 0.3, progress) * (1.0 - smoothstep(0.7, 1.0, progress));
    COLOR.a = fade;
} 