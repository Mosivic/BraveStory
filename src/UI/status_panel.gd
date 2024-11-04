extends HBoxContainer

@export var hp:int = 100
@export var max_hp:int = 100

@onready var heath_bar:TextureProgressBar = $HeathBar
@onready var ease_heath_bar:TextureProgressBar = $HeathBar/EaseHealthBar


func update_health()->void:
	var percentage := hp/float(max_hp)
	heath_bar.value = percentage
		
	create_tween().tween_property(ease_heath_bar,"value",percentage,0.3)
