extends Area2D

signal interacted

func _init() -> void:
	collision_layer = 0
	collision_mask = 0
	set_collision_mask_value(2,true)
	
	

func interact()->void:
	print("[Interact] %s" % name)
	interacted.emit()
	
