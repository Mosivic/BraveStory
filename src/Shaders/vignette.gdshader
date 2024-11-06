shader_type canvas_item;

uniform float radius : hint_range(0.0, 1.0) = 0.7;
uniform float softness : hint_range(0.0, 1.0) = 0.45;
uniform vec4 vignette_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);

void fragment() {
    vec2 uv = UV;
    vec2 center = vec2(0.5, 0.5);
    float dist = distance(uv, center);

    float vignette = smoothstep(radius, radius - softness, dist);

    COLOR = vec4(vignette_color.rgb, vignette_color.a * (1.0 - vignette));
}