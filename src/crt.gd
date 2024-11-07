extends CanvasLayer

var shader_material:ShaderMaterial
var color_rect:ColorRect
var time:float

func _ready() -> void:
	color_rect = get_node("ColorRect")
	shader_material = color_rect.material;


func _process(delta: float) -> void:
	time += delta
	shader_material.set_shader_parameter('time_offset',time)
