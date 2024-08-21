extends Control

@export var first_cut_scene: PackedScene;

func _ready():
	%"Start".grab_focus()

func start_game():
	WorldState.reset()
	WorldState.transition_to_cut_scene(first_cut_scene)

func _on_start_pressed():
	start_game()
	queue_free()
