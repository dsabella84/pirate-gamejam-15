class_name ComicCutScene
extends Control

@export var next_level: PackedScene;
@export var spawn_index: int = 0
@export var spawn_direction: Vector2 = Vector2.DOWN

func transition_to_level():
	WorldState.reset()
	WorldState.transit_player_to_scene(next_level, spawn_index, spawn_direction)

func _on_animation_player_animation_finished(anim_name):
	transition_to_level()	
	queue_free()
