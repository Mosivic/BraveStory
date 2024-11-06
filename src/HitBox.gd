class_name HitBox
extends Area2D

signal hit(hurtbox)


func _init() -> void:
	area_entered.connect(_on_area_entered)


func _on_area_entered(hurtbox : HurtBox):
	print("[Hit] %s => %s" % [owner.name,hurtbox.owner.name])
	hit.emit(hurtbox)
	hurtbox.hurt.emit(self)