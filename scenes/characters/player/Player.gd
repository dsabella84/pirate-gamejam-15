class_name Player
extends CharacterBody2D

var input_movement_vector: Vector2 = Vector2.ZERO;
var is_sprinting: bool = false;

@export var walk_speed: float = 5;
@export var sprint_speed: float = 12;
@onready var _interaction_area: Area2D = $"InteractionArea";

var _sprint_to_walk_ratio:
	get: return sprint_speed / walk_speed;

@onready var _animation_tree: AnimationTree = $"AnimationTree";
@onready var _animation_player: AnimationPlayer = $"AnimationPlayer";

func _physics_process(_delta) -> void:
	var multiplier = sprint_speed if is_sprinting else walk_speed;
	velocity = input_movement_vector * multiplier;
	move_and_collide(velocity);
	_set_animations();

func try_interact() -> Node2D:
	var _sort_interactables_by_dist = func(a: Interactable, b: Interactable) -> bool:
		var lensq_a = (a.position - position).length_squared();
		var lensq_b = (b.position - position).length_squared();
		return lensq_a <= lensq_b;

	var areas = _interaction_area.get_overlapping_areas();
	if len(areas) == 0:
		return null;

	var interactables: Array[Interactable] = areas.map(func(area): return area.get_parent() as Interactable).filter(func(i): return i != null);
	interactables.sort_custom(_sort_interactables_by_dist);
	
	var closest_interactable = interactables[0];
	closest_interactable.interact(self);
	return closest_interactable.get_parent();

func _set_animations():
	var is_walking = input_movement_vector != Vector2.ZERO;
	_animation_tree["parameters/conditions/is_idle"] = !is_walking;
	_animation_tree["parameters/conditions/is_walking"] = is_walking;

	_animation_player.speed_scale = 1 if !is_walking||!is_sprinting else _sprint_to_walk_ratio;

	# preserve old blend position when player stopped
	if !is_walking: return ;
	
	var x = input_movement_vector.x;
	_animation_tree["parameters/idle/blend_position"] = x;
	_animation_tree["parameters/walking/blend_position"] = x;