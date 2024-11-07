shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform vec2 sun_position = vec2(0.5, 0.0);  // 太阳位置
uniform float sun_size : hint_range(0.0, 0.5) = 0.1;  // 太阳大小
uniform float ray_intensity : hint_range(0.0, 2.0) = 0.5;  // 光线强度
uniform float ray_density : hint_range(1.0, 20.0) = 8.0;  // 光线密度
uniform float ray_length : hint_range(0.0, 2.0) = 0.8;  // 光线长度
uniform vec4 sun_color : source_color = vec4(1.0, 0.9, 0.7, 1.0);  // 阳光颜色
uniform float time_offset;  // 时间变量，用于动画

// 随机数生成
float rand(vec2 co) {
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

// 光线函数
float ray(vec2 uv, vec2 pos, float size) {
    vec2 delta = uv - pos;
    float angle = atan(delta.y, delta.x);
    float distance = length(delta);
    
    // 创建光线
    float rays = sin(angle * ray_density + time_offset) * 0.5 + 0.5;
    rays *= exp(-distance * (1.0 - ray_length));
    
    // 添加随机变化
    float noise = rand(vec2(angle * 0.1, time_offset * 0.1)) * 0.1;
    rays += noise;
    
    return rays * ray_intensity;
}

// 光晕函数
float halo(vec2 uv, vec2 pos, float size) {
    float d = length(uv - pos);
    return exp(-d * (1.0/size));
}

void fragment() {
    vec2 uv = SCREEN_UV;
    vec4 screen_color = texture(SCREEN_TEXTURE, SCREEN_UV);
    
    // 计算太阳和光线
    float sun = halo(uv, sun_position, sun_size);
    float rays = ray(uv, sun_position, sun_size);
    
    // 混合光线效果
    vec3 sun_effect = sun_color.rgb * (sun + rays);
    
    // 应用大气散射
    float scatter = 1.0 - uv.y; // 越靠近顶部散射越少
    scatter = pow(scatter, 2.0);
    
    // 最终颜色混合
    vec3 final_color = screen_color.rgb + sun_effect * scatter;
    
    // 添加一些大气微粒效果
    float dust = rand(uv + vec2(time_offset * 0.1)) * 0.05 * scatter;
    final_color += dust * sun_color.rgb;
    
    COLOR = vec4(final_color, 1.0);
} 