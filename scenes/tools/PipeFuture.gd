extends Node2D

var dialog = load("res://scenes/dialogue/SewageValve.dialogue") as DialogueResource

func _on_interactable_interacted(_initiator:Node):
	#if WorldState.in_past: return;
	print('1')
	DialogState.start_dialog(dialog, "try_valve_present")
