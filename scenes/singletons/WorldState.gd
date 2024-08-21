extends Node

signal time_changed(to_future)

var player: Player;
var in_future: bool = true;
var disable_movement: bool = false;
var saved_level_states: Dictionary = {}

var star_fragment_count: int = 0
var policeman_moved: bool = false
var town1_gate_open: bool = false
var newspaper_picked_up: bool = false
var password_found: bool = false
var sewage_valve_off: bool = false
var police_before_newspaper: bool = false #whether the player talked to the policeman before picking up the newspaper
var got_fuse_from_clubhouse: bool = false
var fuse_scene_packed: PackedScene = preload('res://scenes/world/lantern_fuse.tscn')
var scene_transition_visual_packed: PackedScene = preload('res://scenes/ui/SceneTransitionVisual.tscn')
var scene_transition_visual: Node;
var dialog_resource: DialogueResource = preload('res://scenes/dialogue/EndDemo.dialogue')

var _processing_lantern_animation: bool = false;
var _lantern_animation_elapsed: float = 0;
var _lantern_total_duration: float = 0;
var _lantern_level: Level;
var _lantern_theme_switched: bool = false;
var lantern_move_allow_timer: float = 0;

var lantern_unlocked: bool:
	get: 
		var level = get_current_level();
		return !level || !level.lanter_locked;

func _ready():
	scene_transition_visual = scene_transition_visual_packed.instantiate()
	add_child(scene_transition_visual)

func _physics_process(delta):
	if !_processing_lantern_animation: return ;

	_lantern_animation_elapsed += delta;
	lantern_move_allow_timer -= delta;

	if lantern_move_allow_timer <= 0:
		disable_movement = false;
		
	var percent = _lantern_animation_elapsed / _lantern_total_duration;

	if percent >= 1:
		_lantern_level.modulate_nodes(1);
		_lantern_level.bg_color.color = _lantern_level.future_color if in_future else _lantern_level.past_color;

		disable_movement = false;
		DialogState.disabled = false
		time_changed.emit(in_future);
		_processing_lantern_animation = false;
	
	elif percent >= 0.75 && percent < 1:
		_lantern_level.modulate_nodes((percent - 0.75) * 4);

	elif percent >= 0.25 && percent < 0.75:
		var lerp_value = 2 * percent - 0.5 if in_future else 1.5 - 2 * percent;
		_lantern_level.bg_color.color = lerp(_lantern_level.past_color, _lantern_level.future_color, lerp_value);
		if !_lantern_theme_switched:
			_lantern_level.switch_nodes(in_future);
			SoundPlayer.set_time(in_future, 1)
			player.set_collision_masks_and_layers(in_future)
			_lantern_theme_switched = true;
	
	elif percent >= 0:
		_lantern_level.modulate_nodes(1 - 4 * percent);
	
func set_time(to_future: bool) -> void:
	in_future = to_future;

	disable_movement = true;
	DialogState.disabled = true
	_processing_lantern_animation = true;
	_lantern_total_duration = player.play_lantern_animation();
	lantern_move_allow_timer = _lantern_total_duration - 0.5; 
	_lantern_level = get_current_level();
	_lantern_theme_switched = false;
	_lantern_animation_elapsed = 0;

func transit_player_to_scene(destination: PackedScene, spawn_id: int, player_direction: Vector2):
	disable_movement = true
	DialogState.disabled = true
	scene_transition_visual.start()
	
	var level = load_level_state(destination);
	if !(level is Level):
		assert(false, "Invalid destination scene provided, should extend class 'Level'");
	
	await scene_transition_visual.halfway
	
	var current = get_current_level();
	if current:
		save_level_state(current)
		current.visible = false;
		current.queue_free();
	SoundPlayer.stop_all_sfx()
	
	level.spawn_player_at(spawn_id, player_direction);
	get_tree().root.call_deferred("add_child", level);
	level.set_deferred("owner", get_tree().root);
	
	await scene_transition_visual.finished
	
	disable_movement = false
	DialogState.disabled = false

func transition_to_cut_scene(cut_scene: PackedScene):
	disable_movement = true
	DialogState.disabled = true
	scene_transition_visual.start()
	
	var comic_cut_scene = cut_scene.instantiate() as ComicCutScene;
	
	await scene_transition_visual.halfway
	
	var current = get_current_level();
	if current:
		save_level_state(current)
		current.visible = false;
		current.queue_free();
	SoundPlayer.stop_all_sfx()
	
	get_tree().root.call_deferred("add_child", comic_cut_scene);
	
	await scene_transition_visual.finished
	
	disable_movement = false
	DialogState.disabled = false

func get_current_level() -> Level:
	var children = get_tree().root.get_children();
	for child in children:
		if child is Level && !child.is_queued_for_deletion() && child.visible: return child;
	
	return null;

func save_level_state(level: Level):
	var packed_level = PackedScene.new()
	packed_level.pack(level)
	saved_level_states[level.name] = packed_level

func load_level_state(packed_level: PackedScene) -> Level:
	var level = packed_level.instantiate() as Level;
	var _name = level.name;

	if saved_level_states.has(_name):
		level.free();
		return saved_level_states[_name].instantiate() as Level;

	return level;

func start_clubhouse_fuse_scene():
	if !got_fuse_from_clubhouse:
		start_fuse_scene();
		got_fuse_from_clubhouse = true;
	# @todo possibly add some other responce if they have already got the fuse?

func start_fuse_scene():
	if DialogState.balloon: DialogState.balloon.queue_free()
	
	disable_movement = true
	var fuse_scene = fuse_scene_packed.instantiate()
	get_current_level().add_child(fuse_scene)
	await fuse_scene.tree_exited
	
	if star_fragment_count >= 5:
		DialogState.start_dialog(dialog_resource, 'end_demo')
	disable_movement = false

func reset():
	in_future = true;
	saved_level_states.clear()
	star_fragment_count = 0;
	policeman_moved = false;
	town1_gate_open = false;
	newspaper_picked_up = false;
	password_found = false;
	sewage_valve_off = false;
