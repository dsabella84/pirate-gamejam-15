extends StaticBody2D

signal interacted(initiator)

@onready var interactable = $Interactable
@export var canvas_layer: CanvasLayer;

func _ready():
	if canvas_layer:
		canvas_layer.visible = false
	
	if not interactable.is_connected("interacted", Callable(self, "_on_interactable_interacted")):
		interactable.connect("interacted", Callable(self, "_on_interactable_interacted"))

func _on_interactable_interacted(initiator):
	if initiator is Player:
		if WorldState.in_future:
			show_overlay()
			WorldState.password_found = true
		else:
			DialogState.start_dialog(load('res://scenes/dialogue/Clubhouse.dialogue'), 'try_partygoers_house_past')

func show_overlay():
	if canvas_layer:
		canvas_layer.visible = true

func _input(event):
	if canvas_layer && canvas_layer.visible && _is_movement_action(event):
		hide_overlay()

func hide_overlay():
		canvas_layer.visible = false

func _is_movement_action(event: InputEvent) -> bool: 
	var action_names = ["move_up", "move_down", "move_left", "move_right"];
	return action_names.any(func(_n): return event.get_action_strength(_n) > 0);

func _on_wrong_side_interacted(initiator):
	if initiator is Player:
		if WorldState.in_future:
			DialogState.start_dialog(load('res://scenes/dialogue/Clubhouse.dialogue'), 'try_partygoers_house_future')
		else:
			DialogState.start_dialog(load('res://scenes/dialogue/Clubhouse.dialogue'), 'try_partygoers_house_past')
